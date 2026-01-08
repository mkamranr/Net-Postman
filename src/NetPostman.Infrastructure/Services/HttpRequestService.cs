using System.Net.Http;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NetPostman.Core.Interfaces;

namespace NetPostman.Infrastructure.Services;

/// <summary>
/// Implementation of HTTP request execution service
/// </summary>
public class HttpRequestService : IHttpRequestService
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ILogger<HttpRequestService> _logger;

    public HttpRequestService(IHttpClientFactory httpClientFactory, ILogger<HttpRequestService> logger)
    {
        _httpClientFactory = httpClientFactory;
        _logger = logger;
    }

    public async Task<HttpResponseMessage> ExecuteRequestAsync(HttpRequestMessage request)
    {
        var client = _httpClientFactory.CreateClient();
        return await client.SendAsync(request);
    }

    public async Task<RequestExecutionResult> ExecuteRequestAsync(RequestExecutionRequest request)
    {
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        var result = new RequestExecutionResult();

        try
        {
            // Resolve variables in URL and body
            var resolvedUrl = ResolveVariables(request.Url, request.Variables);
            var resolvedBody = request.Body != null ? ResolveVariables(request.Body, request.Variables) : null;

            using var httpRequest = new HttpRequestMessage(new HttpMethod(request.Method), resolvedUrl);

            // Add headers
            foreach (var header in request.Headers.Where(h => h.Enabled))
            {
                var resolvedKey = ResolveVariables(header.Key, request.Variables);
                var resolvedValue = ResolveVariables(header.Value, request.Variables);
                
                // Check if header is a content-type header
                if (resolvedKey.Equals("Content-Type", StringComparison.OrdinalIgnoreCase))
                {
                    httpRequest.Content = new StringContent(resolvedBody ?? "", Encoding.UTF8, resolvedValue);
                }
                else
                {
                    httpRequest.Headers.TryAddWithoutValidation(resolvedKey, resolvedValue);
                }
            }

            // Add body if present and not already added
            if (resolvedBody != null && request.BodyType != null && 
                !httpRequest.Content.Headers.ContentType?.MediaType?.Contains("application/json") == true)
            {
                var contentType = GetContentType(request.BodyType);
                if (httpRequest.Content == null)
                {
                    httpRequest.Content = new StringContent(resolvedBody, Encoding.UTF8, contentType);
                }
            }

            var client = _httpClientFactory.CreateClient();
            client.Timeout = TimeSpan.FromSeconds(request.TimeoutSeconds);

            var response = await client.SendAsync(httpRequest);
            
            stopwatch.Stop();

            result.StatusCode = (int)response.StatusCode;
            result.StatusText = response.ReasonPhrase;
            result.IsSuccess = response.IsSuccessStatusCode;
            result.ResponseTime = stopwatch.ElapsedMilliseconds;

            // Read headers
            foreach (var header in response.Headers)
            {
                result.Headers[header.Key] = string.Join(", ", header.Value);
            }

            // Read body
            var bodyContent = await response.Content.ReadAsStringAsync();
            result.Body = bodyContent;
            result.ResponseSize = System.Text.Encoding.UTF8.GetByteCount(bodyContent);

            // Extract cookies from Set-Cookie header if present
            if (response.Headers.TryGetValues("Set-Cookie", out var cookies))
            {
                foreach (var cookie in cookies)
                {
                    var cookieParts = cookie.Split(';');
                    var nameValue = cookieParts[0].Split('=');
                    if (nameValue.Length == 2)
                    {
                        result.Cookies.Add(new Cookie
                        {
                            Name = nameValue[0].Trim(),
                            Value = nameValue[1].Trim(),
                            Domain = httpRequest.RequestUri?.Host ?? "",
                            Secure = cookieParts.Any(p => p.Trim().Equals("Secure", StringComparison.OrdinalIgnoreCase)),
                            HttpOnly = cookieParts.Any(p => p.Trim().Equals("HttpOnly", StringComparison.OrdinalIgnoreCase))
                        });
                    }
                }
            }
        }
        catch (TaskCanceledException ex) when (ex.CancellationToken != null)
        {
            result.ErrorMessage = "Request timed out";
            result.StatusCode = 0;
            result.IsSuccess = false;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error executing request");
            result.ErrorMessage = ex.Message;
            result.StatusCode = 0;
            result.IsSuccess = false;
        }

        return result;
    }

    private string ResolveVariables(string input, Dictionary<string, string> variables)
    {
        if (string.IsNullOrEmpty(input)) return input;

        foreach (var variable in variables)
        {
            input = input.Replace($"{{{{{variable.Key}}}}}", variable.Value, StringComparison.OrdinalIgnoreCase);
        }

        return input;
    }

    private string GetContentType(string bodyType)
    {
        return bodyType.ToLowerInvariant() switch
        {
            "json" => "application/json",
            "xml" => "application/xml",
            "html" => "text/html",
            "text" => "text/plain",
            "javascript" => "application/javascript",
            "form-data" or "urlencoded" => "application/x-www-form-urlencoded",
            _ => "text/plain"
        };
    }
}
