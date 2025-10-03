using System.Net.Http.Headers;
using Microsoft.AspNetCore.Http;

namespace Infrastructure.HttpClient.Handlers;

public class AuthHeaderHandler : DelegatingHandler
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public AuthHeaderHandler(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    protected override async Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request,
        CancellationToken cancellationToken)
    {
        try
        {
            HttpContext? context = _httpContextAccessor.HttpContext;
            if (context != null && context.Request.Headers.TryGetValue("token", out Microsoft.Extensions.Primitives.StringValues tokenValues))
            {
                string? token = tokenValues.FirstOrDefault();
                if (!string.IsNullOrWhiteSpace(token))
                {
                    // Remove any existing header with same name to avoid duplicates
                    if (request.Headers.Contains("token"))
                    {
                        request.Headers.Remove("token");
                    }

                    request.Headers.Add("token", token);
                }
            }
        }
        catch
        {
            // If anything goes wrong reading the incoming context, don't block the outgoing request.
        }

        return await base.SendAsync(request, cancellationToken).ConfigureAwait(false);
    }
}