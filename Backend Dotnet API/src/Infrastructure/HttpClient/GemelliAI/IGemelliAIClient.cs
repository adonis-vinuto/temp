using Infrastructure.Contracts.GemelliAI.Request;
using Infrastructure.Contracts.GemelliAI.Response;
using Refit;

namespace Infrastructure.HttpClient.GemelliAI;

public interface IGemelliAIClient
{
    [Post("/chat/")]
    Task<ChatResponse> ChatAsync(
        [Body] ChatRequest request,
        CancellationToken cancellationToken = default);

    [Multipart]
    [Post("/file/{organization}/{id_agent}/{id_file}")]
    Task<FileProcessingResponse> FileAsync(
        [AliasAs("file")] StreamPart file,
        string organization,
        string id_agent,
        string id_file,
        CancellationToken cancellationToken = default);

    [Get("/chat/title/{id_session}")]
    Task<string> GetChatTitleAsync(
        string id_session,
        CancellationToken cancellationToken = default);
}
