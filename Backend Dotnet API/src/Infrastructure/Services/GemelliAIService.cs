using Application.Interfaces.Services;
using ErrorOr;
using Infrastructure.Contracts.GemelliAI.Request;
using Infrastructure.Contracts.GemelliAI.Response;
using Infrastructure.HttpClient.GemelliAI;
using Microsoft.Extensions.Logging;
using Refit;
using System.Collections.Generic;
using System.Linq;

namespace Infrastructure.Services;

public class GemelliAIService : IGemelliAIService
{
    private readonly IGemelliAIClient _client;
    private readonly ILogger<GemelliAIService> _logger;

    public GemelliAIService(
        IGemelliAIClient client,
        ILogger<GemelliAIService> logger)
    {
        _client = client;
        _logger = logger;
    }

    public async Task<ErrorOr<GemelliAIChatResponse>> ChatAsync(
        GemelliAIChatRequest request,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var apiRequest = new ChatRequest
            {
                IdSession = string.IsNullOrWhiteSpace(request.IdSession) ? null : request.IdSession,
                IdAgent = request.IdAgent,
                Message = request.Message,
                Module = request.Module,
                Organization = request.Organization,
                User = new ChatUser
                {
                    Name = request.User.Name,
                    Email = request.User.Email
                },
                Preferences = request.Preferences ?? new Dictionary<string, string>(),
                Documents = request.Documents?.Where(documentId => !string.IsNullOrWhiteSpace(documentId)).ToList() ?? new List<string>(),
                AgentType = request.AgentType
            };

            ChatResponse response = await _client.ChatAsync(apiRequest, cancellationToken);

            return new GemelliAIChatResponse
            {
                IdSession = response.IdSession,
                MessageResponse = response.MessageResponse,
                Usage = new GemelliAIChatUsage
                {
                    UsageBreakdownByModel = response.Usage?.UsageBreakdownByModel?.ToDictionary(
                        usage => usage.Key,
                        usage => new GemelliAIChatUsageBreakdown
                        {
                            InputTokens = usage.Value.InputTokens,
                            OutputTokens = usage.Value.OutputTokens,
                            TotalTokens = usage.Value.TotalTokens
                        }) ?? new Dictionary<string, GemelliAIChatUsageBreakdown>(),
                    GrandTotalUsage = response.Usage?.GrandTotalUsage is { } grandTotal
                        ? new GemelliAIChatUsageBreakdown
                        {
                            InputTokens = grandTotal.InputTokens,
                            OutputTokens = grandTotal.OutputTokens,
                            TotalTokens = grandTotal.TotalTokens
                        }
                        : new GemelliAIChatUsageBreakdown()
                }
            };
        }
        catch (ApiException ex)
        {
            _logger.LogError(ex, "Erro ao chamar IA Chat API - Status: {StatusCode}", ex.StatusCode);
            return Error.Failure("IA.Chat.Error", $"Erro na API: {ex.Message}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro inesperado ao chamar IA Chat API");
            return Error.Failure("IA.Chat.Error", "Erro inesperado ao processar requisição");
        }
    }

    public async Task<ErrorOr<GemelliAIFileResponse>> FileAsync(
        GemelliAIFileRequest request,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var filePart = new StreamPart(request.FileStream, request.FileName, "application/octet-stream");

            FileProcessingResponse response = await _client.FileAsync(filePart, request.Organization ?? string.Empty, request.IdAgent ?? string.Empty, cancellationToken);

            return new GemelliAIFileResponse
            {
                FileName = response.FileInfo?.FileName ?? string.Empty,
                Resume = response.FileInfo?.Resume ?? string.Empty
            };
        }
        catch (ApiException ex)
        {
            _logger.LogError(ex, "Erro ao chamar IA File API - Status: {StatusCode}", ex.StatusCode);
            return Error.Failure("IA.File.Error", $"Erro na API: {ex.Message}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro inesperado ao chamar IA File API");
            return Error.Failure("IA.File.Error", "Erro inesperado ao processar requisição");
        }
    }
}
