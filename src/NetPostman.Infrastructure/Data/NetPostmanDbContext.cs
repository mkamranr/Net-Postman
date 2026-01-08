using Microsoft.EntityFrameworkCore;
using NetPostman.Core.Entities;

namespace NetPostman.Infrastructure.Data;

/// <summary>
/// Database context for NetPostman application
/// </summary>
public class NetPostmanDbContext : DbContext
{
    public NetPostmanDbContext(DbContextOptions<NetPostmanDbContext> options) : base(options)
    {
    }
    
    public DbSet<Workspace> Workspaces { get; set; } = null!;
    public DbSet<Collection> Collections { get; set; } = null!;
    public DbSet<Request> Requests { get; set; } = null!;
    public DbSet<Environment> Environments { get; set; } = null!;
    public DbSet<RequestHistory> RequestHistories { get; set; } = null!
;
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        
        // Workspace configuration
        modelBuilder.Entity<Workspace>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.Name);
            entity.HasMany(e => e.Collections)
                  .WithOne(c => c.Workspace)
                  .HasForeignKey(c => c.WorkspaceId)
                  .OnDelete(DeleteBehavior.Cascade);
            entity.HasMany(e => e.Environments)
                  .WithOne(e => e.Workspace)
                  .HasForeignKey(e => e.WorkspaceId)
                  .OnDelete(DeleteBehavior.Cascade);
        });
        
        // Collection configuration
        modelBuilder.Entity<Collection>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.Name);
            entity.HasOne(e => e.Workspace)
                  .WithMany(w => w.Collections)
                  .HasForeignKey(e => e.WorkspaceId)
                  .OnDelete(DeleteBehavior.Cascade);
            entity.HasOne(e => e.Parent)
                  .WithMany(c => c.SubCollections)
                  .HasForeignKey(e => e.ParentId)
                  .OnDelete(DeleteBehavior.Restrict);
            entity.HasMany(e => e.Requests)
                  .WithOne(r => r.Collection)
                  .HasForeignKey(r => r.CollectionId)
                  .OnDelete(DeleteBehavior.Cascade);
        });
        
        // Request configuration
        modelBuilder.Entity<Request>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.Name);
            entity.HasOne(e => e.Collection)
                  .WithMany(c => c.Requests)
                  .HasForeignKey(e => e.CollectionId)
                  .OnDelete(DeleteBehavior.Cascade);
        });
        
        // Environment configuration
        modelBuilder.Entity<Environment>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => new { e.WorkspaceId, e.Name }).IsUnique();
            entity.HasOne(e => e.Workspace)
                  .WithMany(w => w.Environments)
                  .HasForeignKey(e => e.WorkspaceId)
                  .OnDelete(DeleteBehavior.Cascade);
        });
        
        // RequestHistory configuration
        modelBuilder.Entity<RequestHistory>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.ExecutedAt);
            entity.HasOne(e => e.Workspace)
                  .WithMany()
                  .HasForeignKey(e => e.WorkspaceId)
                  .OnDelete(DeleteBehavior.Cascade);
            entity.HasOne(e => e.Request)
                  .WithMany()
                  .HasForeignKey(e => e.RequestId)
                  .OnDelete(DeleteBehavior.SetNull);
        });
    }
}
