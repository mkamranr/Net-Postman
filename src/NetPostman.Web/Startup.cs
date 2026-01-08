using Microsoft.EntityFrameworkCore;
using NetPostman.Core.Interfaces;
using NetPostman.Infrastructure.Data;
using NetPostman.Infrastructure.Repositories;
using NetPostman.Infrastructure.Services;

namespace NetPostman.Web
{
    public class Startup
    {
        public IConfiguration Configuration { get; }

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            // Add controllers with views
            services.AddControllersWithViews()
                .AddJsonOptions(options =>
                {
                    options.JsonSerializerOptions.PropertyNamingPolicy = null;
                    options.JsonSerializerOptions.PropertyNameCaseInsensitive = true;
                });

            // Configure DbContext with SQLite
            services.AddDbContext<NetPostmanDbContext>(options =>
                options.UseSqlite(Configuration.GetConnectionString("DefaultConnection")));

            // Register repositories
            services.AddScoped<IWorkspaceRepository, WorkspaceRepository>();
            services.AddScoped<ICollectionRepository, CollectionRepository>();
            services.AddScoped<IEnvironmentRepository, EnvironmentRepository>();
            services.AddScoped<IRequestHistoryRepository, RequestHistoryRepository>();

            // Register services
            services.AddScoped<IHttpRequestService, HttpRequestService>();

            // Configure HttpClient
            services.AddHttpClient("ProxyClient", client =>
            {
                client.Timeout = TimeSpan.FromSeconds(Configuration.GetValue<int>("HttpClient:TimeoutSeconds", 60));
            });
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, DatabaseInitializer initializer)
        {
            // Initialize database
            initializer.InitializeAsync().Wait();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseStaticFiles();
            app.UseRouting();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
