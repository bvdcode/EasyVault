using EasyVault.Server.Database;
using EasyVault.Server.Services;
using EasyVault.Server.Healthchecks;
using EasyExtensions.AspNetCore.Extensions;
using EasyExtensions.EntityFrameworkCore.Extensions;
using EasyExtensions.EntityFrameworkCore.HealthChecks;
using EasyExtensions.EntityFrameworkCore.Npgsql.Extensions;

namespace EasyVault.Server
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            string[] corsOrigins = builder.Configuration.GetSection("AllowedOrigins").Get<string[]>()
                ?? throw new ArgumentNullException(null, "Allowed origins cannot be null.");
            builder.Services.AddControllers();
            builder.Services.AddPostgresDbContext<AppDbContext>(builder.Configuration)
                .AddDefaultCorsWithOrigins(corsOrigins)
                .AddSingleton<IVault, MemoryVaultService>()
                .AddHealthChecks()
                .AddCheck<SealingHealthCheck>("Vault")
                .AddCheck<DatabaseHealthCheck<AppDbContext>>("Database");

            var app = builder.Build();
            app.UseCors().UseDefaultFiles();
            app.MapStaticAssets();
            app.UseAuthorization();
            app.MapControllers();
            app.MapFallbackToFile("/index.html");
            app.ApplyMigrations<AppDbContext>();
            app.MapHealthChecks("/api/{version}/health");
            app.Run();
        }
    }
}