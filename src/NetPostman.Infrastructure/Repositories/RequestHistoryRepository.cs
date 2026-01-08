using Microsoft.EntityFrameworkCore;
using NetPostman.Core.Entities;
using NetPostman.Core.Interfaces;

namespace NetPostman.Infrastructure.Repositories;

/// <summary>
/// Implementation of request history repository
/// </summary>
public class RequestHistoryRepository : IRequestHistoryRepository
{
    private readonly NetPostmanDbContext _context;

    public RequestHistoryRepository(NetPostmanDbContext context)
    {
        _context = context;
    }

    public async Task<RequestHistory> AddAsync(RequestHistory history)
    {
        history.ExecutedAt = DateTime.UtcNow;
        _context.RequestHistories.Add(history);
        await _context.SaveChangesAsync();
        return history;
    }

    public async Task<IEnumerable<RequestHistory>> GetByWorkspaceIdAsync(Guid workspaceId, int limit = 50)
    {
        return await _context.RequestHistories
            .Where(h => h.WorkspaceId == workspaceId)
            .OrderByDescending(h => h.ExecutedAt)
            .Take(limit)
            .ToListAsync();
    }

    public async Task<bool> ClearHistoryAsync(Guid workspaceId)
    {
        var entries = await _context.RequestHistories
            .Where(h => h.WorkspaceId == workspaceId)
            .ToListAsync();
        
        if (!entries.Any()) return true;
        
        _context.RequestHistories.RemoveRange(entries);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeleteEntryAsync(Guid id)
    {
        var entry = await _context.RequestHistories.FindAsync(id);
        if (entry == null) return false;
        
        _context.RequestHistories.Remove(entry);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<RequestHistory?> GetByIdAsync(Guid id)
    {
        return await _context.RequestHistories
            .FirstOrDefaultAsync(h => h.Id == id);
    }
}
