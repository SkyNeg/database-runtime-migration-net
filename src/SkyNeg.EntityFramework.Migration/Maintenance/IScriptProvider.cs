namespace SkyNeg.EntityFramework.Migration
{
    public interface IScriptProvider
    {
        /// <summary>
        /// Get max version of the database
        /// </summary>
        /// <returns></returns>
        Version GetMaxVersion();

        /// <summary>
        /// Get update script for specific version
        /// </summary>
        /// <param name="version">Current version of the database</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Update script for current version of the database. Null if current version is up to date</returns>
        Task<UpdateScript?> GetUpdateScriptAsync(Version version, CancellationToken cancellationToken);

        /// <summary>
        /// Get create scripts for database
        /// </summary>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns></returns>
        IAsyncEnumerable<UpdateScript> GetCreateScriptsAsync(CancellationToken cancellationToken);
    }
}
