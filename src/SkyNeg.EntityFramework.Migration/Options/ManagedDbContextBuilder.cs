using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace SkyNeg.EntityFramework.Migration.Options
{
    internal class ManagedDbContextBuilder<TContext> : IManagedDbContextBuilder
        where TContext : DbContext
    {
        public IServiceCollection Services { get; }

        public ManagedDbContextBuilder(IServiceCollection services)
        {
            Services = services;
        }

        public void SetDbContextOptions(Action<DbContextOptionsBuilder> options)
        {
            Services.AddDbContextFactory<TContext>(options);
            Services.AddDbContext<TContext>(options);
        }
    }
}
