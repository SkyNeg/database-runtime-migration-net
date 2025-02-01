using Microsoft.EntityFrameworkCore;

namespace SkyNeg.EntityFramework.Migration
{
    public sealed class RuntimeContext<TContext> : DbContext
        where TContext : DbContext
    {
        public DbSet<ComponentVersion> ComponentVersions { get; set; }

        public RuntimeContext(DbContextOptions<RuntimeContext<TContext>> options) : base(options)
        {
        }
    }
}
