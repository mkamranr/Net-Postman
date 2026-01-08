using Microsoft.EntityFrameworkCore;
using NetPostman.Core.Entities;
using NetPostman.Core.Interfaces;

namespace NetPostman.Infrastructure.Repositories;

/// <summary>
/// Implementation of collection repository
/// </summary>
public class CollectionRepository : ICollectionRepository
{
    private readonly NetPostmanDbContext _context;

    public CollectionRepository(NetPostmanDbContext context)
    {
        _context = context;
    }

    public async Task<Collection?> GetByIdAsync(Guid id)
    {
        return await _context.Collections
            .Include(c => c.Requests)
            .Include(c => c.SubCollections)
            .FirstOrDefaultAsync(c => c.Id == id);
    }

    public async Task<IEnumerable<Collection>> GetByWorkspaceIdAsync(Guid workspaceId)
    {
        return await _context.Collections
            .Where(c => c.WorkspaceId == workspaceId && c.ParentId == null)
            .Include(c => c.Requests)
            .Include(c => c.SubCollections)
            .OrderBy(c => c.Name)
            .ToListAsync();
    }

    public async Task<Collection> CreateAsync(Collection collection)
    {
        collection.CreatedAt = DateTime.UtcNow;
        collection.UpdatedAt = DateTime.UtcNow;
        _context.Collections.Add(collection);
        await _context.SaveChangesAsync();
        return collection;
    }

    public async Task<Collection> UpdateAsync(Collection collection)
    {
        collection.UpdatedAt = DateTime.UtcNow;
        _context.Collections.Update(collection);
        await _context.SaveChangesAsync();
        return collection;
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var collection = await _context.Collections.FindAsync(id);
        if (collection == null) return false;
        
        _context.Collections.Remove(collection);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<Request> AddRequestAsync(Guid collectionId, Request request)
    {
        request.CollectionId = collectionId;
        request.CreatedAt = DateTime.UtcNow;
        request.UpdatedAt = DateTime.UtcNow;
        _context.Requests.Add(request);
        await _context.SaveChangesAsync();
        return request;
    }

    public async Task<Request?> GetRequestByIdAsync(Guid requestId)
    {
        return await _context.Requests
            .Include(r => r.Collection)
            .FirstOrDefaultAsync(r => r.Id == requestId);
    }

    public async Task<Request> UpdateRequestAsync(Request request)
    {
        request.UpdatedAt = DateTime.UtcNow;
        _context.Requests.Update(request);
        await _context.SaveChangesAsync();
        return request;
    }

    public async Task<bool> DeleteRequestAsync(Guid requestId)
    {
        var request = await _context.Requests.FindAsync(requestId);
        if (request == null) return false;
        
        _context.Requests.Remove(request);
        await _context.SaveChangesAsync();
        return true;
    }
}
