using Microsoft.EntityFrameworkCore;
using NetPostman.Core.Entities;
using NetPostman.Core.Interfaces;

namespace NetPostman.Infrastructure.Repositories;

/// <summary>
/// Implementation of environment repository
/// </summary>
public class EnvironmentRepository : IEnvironmentRepository
{
    private readonly NetPostmanDbContext _context;

    public EnvironmentRepository(NetPostmanDbContext context)
    {
        _context = context;
    }

    public async Task<Environment?> GetByIdAsync(Guid id)
    {
        return await _context.Environments
            .FirstOrDefaultAsync(e => e.Id == id);
    }

    public async Task<IEnumerable<Environment>> GetByWorkspaceIdAsync(Guid workspaceId)
    {
        return await _context.Environments
            .Where(e => e.WorkspaceId == workspaceId)
            .OrderBy(e => e.IsGlobal ? 0 : 1)
            .ThenBy(e => e.Name)
            .ToListAsync();
    }

    public async Task<Environment> CreateAsync(Environment environment)
    {
        environment.CreatedAt = DateTime.UtcNow;
        environment.UpdatedAt = DateTime.UtcNow;
        _context.Environments.Add(environment);
        await _context.SaveChangesAsync();
        return environment;
    }

    public async Task<Environment> UpdateAsync(Environment environment)
    {
        environment.UpdatedAt = DateTime.UtcNow;
        _context.Environments.Update(environment);
        await _context.SaveChangesAsync();
        return environment;
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var environment = await _context.Environments.FindAsync(id);
        if (environment == null || environment.IsGlobal) return false;
        
        _context.Environments.Remove(environment);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<Environment?> GetGlobalEnvironmentAsync(Guid workspaceId)
    {
        return await _context.Environments
            .FirstOrDefaultAsync(e => e.WorkspaceId == workspaceId && e.IsGlobal);
    }
}
