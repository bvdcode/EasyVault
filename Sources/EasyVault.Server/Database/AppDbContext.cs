using Microsoft.EntityFrameworkCore;
using EasyExtensions.EntityFrameworkCore.Database;

namespace EasyVault.Server.Database
{
    public class AppDbContext(DbContextOptions options) : AuditedDbContext(options)
    {
        public DbSet<Vault> Vaults { get; set; } = null!;
        public DbSet<AccessEvent> AccessEvents { get; set; } = null!;
    }
}