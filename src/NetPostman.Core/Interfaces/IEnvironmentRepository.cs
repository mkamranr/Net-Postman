namespace NetPostman.Core.Interfaces;

/// <summary>
/// Interface for environment repository operations
/// </summary>
public interface IEnvironmentRepository
{
    Task<Environment?> GetByIdAsync(Guid id);
    Task<IEnumerable<Environment>> GetByWorkspaceIdAsync(Guid workspaceId);
    Task<Environment> CreateAsync(Environment environment);
    Task<Environment> UpdateAsync(Environment environment);
    Task<bool> DeleteAsync(Guid id);
    Task<Environment?> GetGlobalEnvironmentAsync(Guid workspaceId);
}
