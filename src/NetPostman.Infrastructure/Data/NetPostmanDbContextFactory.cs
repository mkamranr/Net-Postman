using Microsoft.EntityFrameworkCore;
using NetPostman.Core.Entities;

namespace NetPostman.Infrastructure.Data;

/// <summary>
/// Database context factory for migrations and design-time operations
/// </summary>
public class NetPostmanDbContextFactory : IDesignTimeDbContextFactory<NetPostmanDbContext>
{
    public NetPostmanDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<NetPostmanDbContext>();
        
        // Default to SQLite for design-time operations
        optionsBuilder.UseSqlite("Data Source=netpostman.db");
        
        return new NetPostmanDbContext(optionsBuilder.Options);
    }
}
