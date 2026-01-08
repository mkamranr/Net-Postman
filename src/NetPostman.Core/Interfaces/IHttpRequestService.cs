namespace NetPostman.Core.Interfaces;

/// <summary>
/// Result of request execution
/// </summary>
public class RequestExecutionResult
{
    public int StatusCode { get; set; }
    public string? StatusText { get; set; }
    public Dictionary<string, string> Headers { get; set; } = new();
    public string? Body { get; set; }
    public long ResponseTime { get; set; }
    public long ResponseSize { get; set; }
    public bool IsSuccess { get; set; }
    public string? ErrorMessage { get; set; }
    public List<Cookie> Cookies { get; set; } = new();
}

public class Cookie
{
    public string Name { get; set; } = string.Empty;
    public string Value { get; set; } = string.Empty;
    public string Domain { get; set; } = string.Empty;
    public string Path { get; set; } = "/";
    public DateTime? Expires { get; set; }
    public bool Secure { get; set; }
    public bool HttpOnly { get; set; }
}

/// <summary>
/// Request configuration for execution
/// </summary>
public class RequestExecutionRequest
{
    public string Method { get; set; } = "GET";
    public string Url { get; set; } = string.Empty;
    public List<KeyValuePair> Headers { get; set; } = new();
    public string? Body { get; set; }
    public string? BodyType { get; set; }
    public Dictionary<string, string> Variables { get; set; } = new();
    public int TimeoutSeconds { get; set; } = 30;
}

public class KeyValuePair
{
    public string Key { get; set; } = string.Empty;
    public string Value { get; set; } = string.Empty;
    public bool Enabled { get; set; } = true;
}

/// <summary>
/// Interface for HTTP request execution service
/// </summary>
public interface IHttpRequestService
{
    /// <summary>
    /// Executes an HTTP request and returns the response
    /// </summary>
    Task<HttpResponseMessage> ExecuteRequestAsync(HttpRequestMessage request);
    
    /// <summary>
    /// Executes a request with the given configuration
    /// </summary>
    Task<RequestExecutionResult> ExecuteRequestAsync(RequestExecutionRequest request);
}
