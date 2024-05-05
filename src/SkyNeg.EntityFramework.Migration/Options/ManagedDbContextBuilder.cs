using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using SkyNeg.EntityFramework.Migration.ScriptProviders;

namespace SkyNeg.EntityFramework.Migration.Options
{
    internal class ManagedDbContextBuilder<TContext> : IManagedDbContextBuilder<TContext>
        where TContext : RuntimeContext
    {
        public IServiceCollection Services { get; }

        public ManagedDbContextBuilder(IServiceCollection services)
        {
            Services = services;
            services.AddSingleton<IDatabaseManager<TContext>, DatabaseManager<TContext>>();
        }

        public void SetDbContextOptions(Action<DbContextOptionsBuilder> options)
        {
            Services.AddDbContextFactory<TContext>(options);
            Services.AddDbContext<TContext>(options);
        }

        public void AddComponentScriptProvider(string component, IScriptProvider scriptProvider)
        {

        }

        public void AddDefaultScriptProvider(IScriptProvider scriptProvider) { }
    }
}
