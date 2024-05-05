using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace SkyNeg.EntityFramework.Migration.Options
{
    public interface IManagedDbContextBuilder<TContext>
        where TContext : RuntimeContext
    {
        IServiceCollection Services { get; }

        void SetDbContextOptions(Action<DbContextOptionsBuilder> options);
    }
}
