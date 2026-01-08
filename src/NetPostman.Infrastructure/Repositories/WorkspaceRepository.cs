using Microsoft.EntityFrameworkCore;
using NetPostman.Core.Entities;
using NetPostman.Core.Interfaces;

namespace NetPostman.Infrastructure.Repositories;

/// <summary>
/// Implementation of workspace repository
/// </summary>
public class WorkspaceRepository : IWorkspaceRepository
{
    private readonly NetPostmanDbContext _context;

    public WorkspaceRepository(NetPostmanDbContext context)
    {
        _context = context;
    }

    public async Task<Workspace?> GetByIdAsync(Guid id)
    {
        return await _context.Workspaces
            .FirstOrDefaultAsync(w => w.Id == id);
    }

    public async Task<IEnumerable<Workspace>> GetAllAsync()
    {
        return await _context.Workspaces
            .Where(w => w.IsActive)
            .OrderBy(w => w.Name)
            .ToListAsync();
    }

    public async Task<Workspace> CreateAsync(Workspace workspace)
    {
        workspace.CreatedAt = DateTime.UtcNow;
        workspace.UpdatedAt = DateTime.UtcNow;
        _context.Workspaces.Add(workspace);
        await _context.SaveChangesAsync();
        return workspace;
    }

    public async Task<Workspace> UpdateAsync(Workspace workspace)
    {
        workspace.UpdatedAt = DateTime.UtcNow;
        _context.Workspaces.Update(workspace);
        await _context.SaveChangesAsync();
        return workspace;
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var workspace = await _context.Workspaces.FindAsync(id);
        if (workspace == null) return false;
        
        workspace.IsActive = false;
        _context.Workspaces.Update(workspace);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<Workspace?> GetDefaultWorkspaceAsync()
    {
        return await _context.Workspaces
            .FirstOrDefaultAsync(w => w.IsActive);
    }
}
