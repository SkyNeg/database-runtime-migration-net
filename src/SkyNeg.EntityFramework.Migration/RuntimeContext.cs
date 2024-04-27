using Microsoft.EntityFrameworkCore;

namespace SkyNeg.EntityFramework.Migration
{
    public class RuntimeContext : DbContext
    {
        public DbSet<ComponentVersion> ComponentVersions { get; set; }

        public RuntimeContext(DbContextOptions<RuntimeContext> options)
        : base(options)
        {
        }
    }
}
