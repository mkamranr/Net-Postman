using System.ComponentModel.DataAnnotations;

namespace NetPostman.Core.Entities;

/// <summary>
/// Represents a collection of requests organized in folders
/// </summary>
public class Collection
{
    [Key]
    public Guid Id { get; set; } = Guid.NewGuid();
    
    [Required]
    public Guid WorkspaceId { get; set; }
    
    [Required]
    [MaxLength(200)]
    public string Name { get; set; } = string.Empty;
    
    [MaxLength(1000)]
    public string? Description { get; set; }
    
    public Guid? ParentId { get; set; }
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    
    // Navigation properties
    [ForeignKey(nameof(WorkspaceId))]
    public virtual Workspace Workspace { get; set; } = null!;
    
    [ForeignKey(nameof(ParentId))]
    public virtual Collection? Parent { get; set; }
    
    public virtual ICollection<Collection> SubCollections { get; set; } = new List<Collection>();
    public virtual ICollection<Request> Requests { get; set; } = new List<Request>();
}
