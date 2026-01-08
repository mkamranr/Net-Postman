var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddControllersWithViews()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.PropertyNamingPolicy = null;
        options.JsonSerializerOptions.PropertyNameCaseInsensitive = true;
    });

// Register application services
builder.Services.AddApplicationServices();

// Configure HttpClient
builder.Services.AddHttpClient("ProxyClient", client =>
{
    client.Timeout = TimeSpan.FromSeconds(builder.Configuration.GetValue<int>("HttpClient:TimeoutSeconds", 60));
});

// Add DbContext with SQLite
builder.Services.AddDbContext<NetPostman.Infrastructure.Data.NetPostmanDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

// Register repositories
builder.Services.AddScoped<NetPostman.Core.Interfaces.IWorkspaceRepository, NetPostman.Infrastructure.Repositories.WorkspaceRepository>();
builder.Services.AddScoped<NetPostman.Core.Interfaces.ICollectionRepository, NetPostman.Infrastructure.Repositories.CollectionRepository>();
builder.Services.AddScoped<NetPostman.Core.Interfaces.IEnvironmentRepository, NetPostman.Infrastructure.Repositories.EnvironmentRepository>();
builder.Services.AddScoped<NetPostman.Core.Interfaces.IRequestHistoryRepository, NetPostman.Infrastructure.Repositories.RequestHistoryRepository>();

// Register services
builder.Services.AddScoped<NetPostman.Core.Interfaces.IHttpRequestService, NetPostman.Infrastructure.Services.HttpRequestService>();

var app = builder.Build();

// Initialize database
using (var scope = app.Services.CreateScope())
{
    var initializer = scope.ServiceProvider.GetRequiredService<NetPostman.Infrastructure.Data.DatabaseInitializer>();
    await initializer.InitializeAsync();
}

// Configure the HTTP request pipeline
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
}

app.UseStaticFiles();
app.UseRouting();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
