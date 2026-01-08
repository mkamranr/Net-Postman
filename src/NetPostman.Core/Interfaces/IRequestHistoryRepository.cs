namespace NetPostman.Core.Interfaces;

/// <summary>
/// Interface for request history repository operations
/// </summary>
public interface IRequestHistoryRepository
{
    Task<RequestHistory> AddAsync(RequestHistory history);
    Task<IEnumerable<RequestHistory>> GetByWorkspaceIdAsync(Guid workspaceId, int limit = 50);
    Task<bool> ClearHistoryAsync(Guid workspaceId);
    Task<bool> DeleteEntryAsync(Guid id);
    Task<RequestHistory?> GetByIdAsync(Guid id);
}
