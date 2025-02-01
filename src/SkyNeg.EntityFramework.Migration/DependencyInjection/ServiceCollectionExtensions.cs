using Microsoft.EntityFrameworkCore;
using SkyNeg.EntityFramework.Migration;
using SkyNeg.EntityFramework.Migration.Options;
using SkyNeg.EntityFramework.Migration.ScriptProviders;
using System.Reflection;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddManagedDbContext<TContext>(this IServiceCollection services, Action<IManagedDbContextBuilder<TContext>> configure, ServiceLifetime lifetime = ServiceLifetime.Singleton)
            where TContext : DbContext
        {
            configure(new ManagedDbContextBuilder<TContext>(services));
            services.AddSingleton<IDatabaseManager<TContext>, DatabaseManager<TContext>>();
            return services;
        }

        public static IManagedDbContextBuilder<TContext> AddResourceScriptProvider<TContext>(this IManagedDbContextBuilder<TContext> builder, string createScriptPrefix = "", string updateScriptPrefix = "", Assembly? sourceAssembly = null)
            where TContext : DbContext
        {
            sourceAssembly = sourceAssembly ?? Assembly.GetCallingAssembly();
            return builder.AddScriptProvider(new ContextScriptProvider<TContext>(new ResourceScriptProvider(sourceAssembly, createScriptPrefix, updateScriptPrefix)));
        }

        public static IManagedDbContextBuilder<TContext> AddScriptProvider<TContext>(this IManagedDbContextBuilder<TContext> builder, IScriptProvider scriptProvider)
            where TContext : DbContext
            => builder.AddScriptProvider((provider) => scriptProvider, ServiceLifetime.Singleton);

        public static IManagedDbContextBuilder<TContext> AddScriptProvider<TContext, TScriptProvider>(this IManagedDbContextBuilder<TContext> builder, Func<IServiceProvider, TScriptProvider> scriptProviderFactory, ServiceLifetime lifetime = ServiceLifetime.Singleton)
            where TContext : DbContext
            where TScriptProvider : IScriptProvider
        {
            builder.Services.Add(new ServiceDescriptor(typeof(IScriptProvider<TContext>), (sp) =>
            {
                return new ContextScriptProvider<TContext>(scriptProviderFactory(sp));
            }, lifetime));
            return builder;
        }
    }
}

