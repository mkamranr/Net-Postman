using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NetPostman.Core.Entities;

/// <summary>
/// Represents an HTTP request with all its configuration
/// </summary>
public class Request
{
    [Key]
    public Guid Id { get; set; } = Guid.NewGuid();
    
    [Required]
    [MaxLength(200)]
    public string Name { get; set; } = string.Empty;
    
    public Guid CollectionId { get; set; }
    
    /// <summary>
    /// HTTP method (GET, POST, PUT, DELETE, PATCH, HEAD, OPTIONS)
    /// </summary>
    [Required]
    [MaxLength(10)]
    public string Method { get; set; } = "GET";
    
    /// <summary>
    /// The request URL
    /// </summary>
    [Required]
    [MaxLength(4000)]
    public string Url { get; set; } = string.Empty;
    
    /// <summary>
    /// JSON string containing query parameters
    /// </summary>
    public string? QueryParamsJson { get; set; }
    
    /// <summary>
    /// JSON string containing request headers in key-value format
    /// </summary>
    public string? HeadersJson { get; set; }
    
    /// <summary>
    /// Request body type: none, form-data, urlencoded, raw, binary, graphql
    /// </summary>
    [MaxLength(20)]
    public string? BodyType { get; set; }
    
    /// <summary>
    /// JSON string containing body content based on body type
    /// </summary>
    public string? BodyJson { get; set; }
    
    /// <summary>
    /// Pre-request script in JavaScript
    /// </summary>
    public string? PreRequestScript { get; set; }
    
    /// <summary>
    /// Test script in JavaScript
    /// </summary>
    public string? TestScript { get; set; }
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    
    // Navigation properties
    [ForeignKey(nameof(CollectionId))]
    public virtual Collection Collection { get; set; } = null!;
}
