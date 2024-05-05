using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using SkyNeg.EntityFramework.Migration.Exceptions;
using SkyNeg.EntityFramework.Migration.Sample.Data;

namespace SkyNeg.EntityFramework.Migration.Sample
{
    internal class DatabaseManagerService : IHostedService
    {
        private readonly IDatabaseManager<SampleContext> _databaseManager;
        private readonly ILogger _logger;
        public DatabaseManagerService(ILogger<DatabaseManagerService> logger, IDatabaseManager<SampleContext> databaseManager)
        {
            _logger = logger;
            _databaseManager = databaseManager;

        }
        public async Task StartAsync(CancellationToken cancellationToken)
        {
            try
            {
                var updateResult = await _databaseManager.UpdateComponentAsync("SampleComponent", cancellationToken);
                if (updateResult.NewVersion != updateResult.InitialVersion)
                {
                    _logger.LogInformation($"Database updated to version {updateResult.NewVersion}. Applied {updateResult.ScriptsApplied} scripts.");
                }
                else
                {
                    _logger.LogInformation($"Database is up to date. Version {updateResult.NewVersion}.");
                }
            }
            catch (DatabaseUpdateException ex)
            {
                _logger.LogCritical($"Can not update database {ex}");
                throw;
            }
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }
}
