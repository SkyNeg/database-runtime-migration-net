using Microsoft.EntityFrameworkCore;

namespace SkyNeg.EntityFramework.Migration
{
    public interface IDatabaseManager<TContext>
        where TContext : DbContext
    {
        /// <summary>
        /// Retrieves database version.
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns>Version of the database</returns>
        Task<Version?> GetVersionAsync(CancellationToken cancellationToken);

        /// <summary>
        /// Retrieves database component version.
        /// </summary>
        /// <param name="component">Name of the component</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns><see cref="Version"/> of the component or null for unknown version</returns>
        Task<Version?> GetComponentVersionAsync(string component, CancellationToken cancellationToken);

        /// <summary>
        /// Update core databse.
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns>New version of the database</returns>
        Task<DatabaseUpdateResult> UpdateAsync(CancellationToken cancellationToken);

        /// <summary>
        /// Update database component.
        /// </summary>
        /// <param name="component"></param>
        /// <param name="cancellationToken"></param>
        /// <returns>New version of the database component</returns>
        Task<DatabaseUpdateResult> UpdateComponentAsync(string component, CancellationToken cancellationToken);
    }
}
