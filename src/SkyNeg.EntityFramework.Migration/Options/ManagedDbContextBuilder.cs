using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace SkyNeg.EntityFramework.Migration.Options
{
    internal class ManagedDbContextBuilder<TContext> : IManagedDbContextBuilder<TContext>
        where TContext : DbContext
    {
        public IServiceCollection Services { get; }

        public ManagedDbContextBuilder(IServiceCollection services)
        {
            Services = services;
            services.AddSingleton<IDatabaseManager<TContext>, DatabaseManager<TContext>>();
        }

        public void SetDbContextOptions(Action<DbContextOptionsBuilder> options)
        {
            Services.AddDbContextFactory<RuntimeContext<TContext>>(options);
            Services.AddDbContextFactory<TContext>(options, ServiceLifetime.Singleton);
            Services.AddDbContext<TContext>(options, ServiceLifetime.Scoped, ServiceLifetime.Scoped);
        }
    }
}
