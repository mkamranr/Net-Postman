namespace NetPostman.Core.Interfaces;

/// <summary>
/// Interface for workspace repository operations
/// </summary>
public interface IWorkspaceRepository
{
    Task<Workspace?> GetByIdAsync(Guid id);
    Task<IEnumerable<Workspace>> GetAllAsync();
    Task<Workspace> CreateAsync(Workspace workspace);
    Task<Workspace> UpdateAsync(Workspace workspace);
    Task<bool> DeleteAsync(Guid id);
    Task<Workspace?> GetDefaultWorkspaceAsync();
}
