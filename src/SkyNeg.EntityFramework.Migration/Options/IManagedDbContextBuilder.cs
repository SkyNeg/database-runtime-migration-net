using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace SkyNeg.EntityFramework.Migration.Options
{
    public interface IManagedDbContextBuilder<TContext>
        where TContext : DbContext
    {
        IServiceCollection Services { get; }

        void SetDbContextOptions(Action<DbContextOptionsBuilder> options);
    }
}
