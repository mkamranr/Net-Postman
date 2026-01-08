using NetPostman.Core.Interfaces;
using NetPostman.Infrastructure.Repositories;
using NetPostman.Infrastructure.Services;

namespace NetPostman.Web.Services;

/// <summary>
/// Dependency injection configuration service
/// </summary>
public static class DependencyInjection
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        // Repositories
        services.AddScoped<IWorkspaceRepository, WorkspaceRepository>();
        services.AddScoped<ICollectionRepository, CollectionRepository>();
        services.AddScoped<IEnvironmentRepository, EnvironmentRepository>();
        services.AddScoped<IRequestHistoryRepository, RequestHistoryRepository>();
        
        // Services
        services.AddScoped<IHttpRequestService, HttpRequestService>();
        
        return services;
    }
}
