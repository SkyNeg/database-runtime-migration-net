using SkyNeg.EntityFramework.Migration;
using SkyNeg.EntityFramework.Migration.Options;
using SkyNeg.EntityFramework.Migration.ScriptProviders;
using System.Reflection;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddManagedDbContext<TContext>(this IServiceCollection services, Action<IManagedDbContextBuilder<TContext>> configure, ServiceLifetime lifetime = ServiceLifetime.Singleton)
            where TContext : RuntimeContext
        {
            configure(new ManagedDbContextBuilder<TContext>(services));
            services.AddSingleton<IDatabaseManager<TContext>, DatabaseManager<TContext>>();
            return services;
        }

        public static IManagedDbContextBuilder<TContext> AddResourceScriptProvider<TContext>(this IManagedDbContextBuilder<TContext> builder, string createScriptPrefix = "", string updateScriptPrefix = "", Assembly? sourceAssembly = null)
            where TContext : RuntimeContext
        {
            sourceAssembly = sourceAssembly ?? Assembly.GetCallingAssembly();
            builder.Services.AddSingleton<IScriptProvider<TContext>, ContextScriptProvider<TContext>>((services) => new ContextScriptProvider<TContext>(new ResourceScriptProvider(sourceAssembly, createScriptPrefix, updateScriptPrefix)));
            return builder;
        }

        public static IManagedDbContextBuilder<TContext> AddScriptProvider<TContext>(this IManagedDbContextBuilder<TContext> builder, IScriptProvider scriptProvider)
            where TContext : RuntimeContext
        {
            builder.Services.AddSingleton<IScriptProvider<TContext>, ContextScriptProvider<TContext>>((services) => new ContextScriptProvider<TContext>(scriptProvider));
            return builder;
        }

        public static IManagedDbContextBuilder<TContext> AddScriptProvider<TContext, TScriptProvider>(this IManagedDbContextBuilder<TContext> builder, Func<IServiceProvider, TScriptProvider> scriptProviderFactory, ServiceLifetime lifetime = ServiceLifetime.Singleton)
            where TContext : RuntimeContext
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

