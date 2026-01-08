namespace NetPostman.Core.Interfaces;

/// <summary>
/// Interface for collection repository operations
/// </summary>
public interface ICollectionRepository
{
    Task<Collection?> GetByIdAsync(Guid id);
    Task<IEnumerable<Collection>> GetByWorkspaceIdAsync(Guid workspaceId);
    Task<Collection> CreateAsync(Collection collection);
    Task<Collection> UpdateAsync(Collection collection);
    Task<bool> DeleteAsync(Guid id);
    Task<Request> AddRequestAsync(Guid collectionId, Request request);
    Task<Request?> GetRequestByIdAsync(Guid requestId);
    Task<Request> UpdateRequestAsync(Request request);
    Task<bool> DeleteRequestAsync(Guid requestId);
}
