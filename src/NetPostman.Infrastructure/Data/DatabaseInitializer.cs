using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NetPostman.Core.Entities;
using NetPostman.Core.Interfaces;

namespace NetPostman.Infrastructure.Data;

/// <summary>
/// Database initializer to create default workspace and seed data
/// </summary>
public class DatabaseInitializer
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<DatabaseInitializer> _logger;

    public DatabaseInitializer(IServiceProvider serviceProvider, ILogger<DatabaseInitializer> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    public async Task InitializeAsync()
    {
        using var scope = _serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<NetPostmanDbContext>();
        
        try
        {
            _logger.LogInformation("Ensuring database is created...");
            await context.Database.EnsureCreatedAsync();
            
            // Check if default workspace exists
            if (!await context.Workspaces.AnyAsync())
            {
                _logger.LogInformation("Creating default workspace...");
                var defaultWorkspace = new Workspace
                {
                    Name = "My Workspace",
                    Description = "Default workspace for API testing"
                };
                
                context.Workspaces.Add(defaultWorkspace);
                
                // Create global environment
                var globalEnv = new Environment
                {
                    WorkspaceId = defaultWorkspace.Id,
                    Name = "Global",
                    IsGlobal = true,
                    VariablesJson = "{}"
                };
                context.Environments.Add(globalEnv);
                
                await context.SaveChangesAsync();
                _logger.LogInformation("Default workspace created successfully");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while initializing the database");
            throw;
        }
    }
}
