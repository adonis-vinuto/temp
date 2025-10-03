using Application.AppConfig;
using Microsoft.Extensions.Options;

namespace Infrastructure.HttpClient.Handlers;

public class AuthHeaderHandler : DelegatingHandler
{
    private readonly string _token;

    public AuthHeaderHandler(IOptions<GemelliAISettings> gemelliAISettings)
    {
        _token = gemelliAISettings.Value.Token ?? string.Empty;
    }

    protected override async Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request,
        CancellationToken cancellationToken)
    {
        if (!string.IsNullOrWhiteSpace(_token))
        {
            if (request.Headers.Contains("token"))
            {
                request.Headers.Remove("token");
            }

            request.Headers.Add("token", _token);
        }

        return await base.SendAsync(request, cancellationToken).ConfigureAwait(false);
    }
}