using Application.AppConfig;
using Microsoft.Extensions.Options;

namespace Infrastructure.HttpClient.Handlers;

public class AuthHeaderHandler : DelegatingHandler
{
    private readonly GemelliAISettings _settings;

    public AuthHeaderHandler(IOptions<GemelliAISettings> options)
    {
        _settings = options.Value;
    }

    protected override async Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request,
        CancellationToken cancellationToken)
    {
        if (!string.IsNullOrWhiteSpace(_settings.Token))
        {
            if (request.Headers.Contains("token"))
            {
                request.Headers.Remove("token");
            }

            request.Headers.Add("token", _settings.Token);
        }

        return await base.SendAsync(request, cancellationToken).ConfigureAwait(false);
    }
}