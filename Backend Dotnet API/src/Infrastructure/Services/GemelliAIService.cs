using Application.Interfaces.Services;
using ErrorOr;
using Infrastructure.Contracts.GemelliAI.Request;
using Infrastructure.Contracts.GemelliAI.Response;
using Infrastructure.HttpClient.GemelliAI;
using Microsoft.Extensions.Logging;
using Refit;

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
                Message = request.Message,
                Module = request.Module.ToString(),
                Organization = request.Organization,
                User = new ChatUser
                {
                    Name = request.UserName,
                    Email = request.UserEmail
                },
                AgentType = request.AgentType,
                Files = request.Files.Select(f => new ChatFile
                {
                    Name = f.Name,
                    Content = f.Content
                }).ToList(),
                ChatHistory = request.ChatHistory.Select(h => new ChatHistoryItem
                {
                    Role = h.Role,
                    Content = h.Content
                }).ToList()
            };

            ChatResponse response = await _client.ChatAsync(apiRequest, cancellationToken);

            return new GemelliAIChatResponse
            {
                MessageResponse = response.MessageResponse,
                ModelName = response.Usage.ModelName,
                TotalTokens = response.Usage.TotalTokens
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
