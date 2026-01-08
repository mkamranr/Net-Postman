using System.ComponentModel.DataAnnotations;

namespace NetPostman.Core.Entities;

/// <summary>
/// Represents an environment with variables for request configuration
/// </summary>
public class Environment
{
    [Key]
    public Guid Id { get; set; } = Guid.NewGuid();
    
    [Required]
    public Guid WorkspaceId { get; set; }
    
    [Required]
    [MaxLength(200)]
    public string Name { get; set; } = string.Empty;
    
    /// <summary>
    /// JSON string containing environment variables in key-value format
    /// </summary>
    public string VariablesJson { get; set; } = "{}";
    
    public bool IsGlobal { get; set; } = false;
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    
    // Navigation properties
    [ForeignKey(nameof(WorkspaceId))]
    public virtual Workspace Workspace { get; set; } = null!;
}
