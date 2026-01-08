using System.ComponentModel.DataAnnotations;

namespace NetPostman.Core.Entities;

/// <summary>
/// Represents a workspace for organizing collections and requests
/// </summary>
public class Workspace
{
    [Key]
    public Guid Id { get; set; } = Guid.NewGuid();
    
    [Required]
    [MaxLength(200)]
    public string Name { get; set; } = string.Empty;
    
    [MaxLength(1000)]
    public string? Description { get; set; }
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    
    public bool IsActive { get; set; } = true;
    
    // Navigation properties
    public virtual ICollection<Collection> Collections { get; set; } = new List<Collection>();
    public virtual ICollection<Environment> Environments { get; set; } = new List<Environment>();
}
