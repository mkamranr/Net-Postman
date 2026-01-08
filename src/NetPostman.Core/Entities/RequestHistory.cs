using System.ComponentModel.DataAnnotations;

namespace NetPostman.Core.Entities;

/// <summary>
/// Represents a history entry for executed requests
/// </summary>
public class RequestHistory
{
    [Key]
    public Guid Id { get; set; } = Guid.NewGuid();
    
    [Required]
    public Guid WorkspaceId { get; set; }
    
    public Guid? RequestId { get; set; }
    
    /// <summary>
    /// HTTP method used
    /// </summary>
    [Required]
    [MaxLength(10)]
    public string Method { get; set; } = "GET";
    
    /// <summary>
    /// URL that was called
    /// </summary>
    [Required]
    [MaxLength(4000)]
    public string Url { get; set; } = string.Empty;
    
    /// <summary>
    /// HTTP status code received
    /// </summary>
    public int? StatusCode { get; set; }
    
    /// <summary>
    /// Status text received
    /// </summary>
    [MaxLength(200)]
    public string? StatusText { get; set; }
    
    /// <summary>
    /// Response time in milliseconds
    /// </summary>
    public long? ResponseTime { get; set; }
    
    /// <summary>
    /// Response size in bytes
    /// </summary>
    public long? ResponseSize { get; set; }
    
    /// <summary>
    /// Cached response body (truncated for large responses)
    /// </summary>
    public string? ResponseBody { get; set; }
    
    /// <summary>
    /// JSON string containing response headers
    /// </summary>
    public string? ResponseHeadersJson { get; set; }
    
    /// <summary>
    /// Request headers that were sent
    /// </summary>
    public string? RequestHeadersJson { get; set; }
    
    /// <summary>
    /// Request body that was sent
    /// </summary>
    public string? RequestBody { get; set; }
    
    public DateTime ExecutedAt { get; set; } = DateTime.UtcNow;
    
    // Navigation properties
    public Guid? CollectionId { get; set; }
    
    [ForeignKey(nameof(WorkspaceId))]
    public virtual Workspace Workspace { get; set; } = null!;
    
    [ForeignKey(nameof(RequestId))]
    public virtual Request? Request { get; set; }
}
