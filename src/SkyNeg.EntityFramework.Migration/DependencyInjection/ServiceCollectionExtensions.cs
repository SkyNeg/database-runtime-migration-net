using Microsoft.EntityFrameworkCore;
using SkyNeg.EntityFramework.Migration;
using SkyNeg.EntityFramework.Migration.Options;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddManagedDbContext<TContext>(this IServiceCollection services, Action<IManagedDbContextBuilder> configure, ServiceLifetime lifetime = ServiceLifetime.Singleton)
            where TContext : DbContext
        {
            configure(new ManagedDbContextBuilder<TContext>(services));
            services.AddSingleton<IDatabaseManager, DatabaseManager>();
            return services;
        }

        public static IManagedDbContextBuilder AddResourceScriptProvider(this IManagedDbContextBuilder builder, string createScriptPrefix = "", string updateScriptPrefix = "")
        {
            var assembly = System.Reflection.Assembly.GetCallingAssembly();
            builder.Services.AddSingleton<IScriptProvider, ResourceScriptProvider>((services) => new ResourceScriptProvider(assembly, createScriptPrefix, updateScriptPrefix));
            return builder;
        }
    }
}

