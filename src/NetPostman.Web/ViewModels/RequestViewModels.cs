namespace NetPostman.Web.ViewModels;

/// <summary>
/// View model for request/response display
/// </summary>
public class RequestViewModel
{
    public string Method { get; set; } = "GET";
    public string Url { get; set; } = string.Empty;
    public List<KeyValueViewModel> Headers { get; set; } = new();
    public List<KeyValueViewModel> QueryParams { get; set; } = new();
    public string? BodyType { get; set; }
    public string? Body { get; set; }
    public string? PreRequestScript { get; set; }
    public string? TestScript { get; set; }
    public Guid? CollectionId { get; set; }
    public Guid? RequestId { get; set; }
    public string? RequestName { get; set; }
}

public class KeyValueViewModel
{
    public string Key { get; set; } = string.Empty;
    public string Value { get; set; } = string.Empty;
    public bool Enabled { get; set; } = true;
    public bool IsActive { get; set; } = true;
}

public class ResponseViewModel
{
    public int StatusCode { get; set; }
    public string? StatusText { get; set; }
    public long ResponseTime { get; set; }
    public long ResponseSize { get; set; }
    public string? Body { get; set; }
    public List<KeyValueViewModel> Headers { get; set; } = new();
    public bool IsSuccess { get; set; }
    public string? ErrorMessage { get; set; }
    public List<CookieViewModel> Cookies { get; set; } = new();
}

public class CookieViewModel
{
    public string Name { get; set; } = string.Empty;
    public string Value { get; set; } = string.Empty;
    public string Domain { get; set; } = string.Empty;
    public string Path { get; set; } = "/";
    public DateTime? Expires { get; set; }
    public bool Secure { get; set; }
    public bool HttpOnly { get; set; }
}
