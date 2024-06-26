﻿using Microsoft.EntityFrameworkCore;
using SkyNeg.EntityFramework.Migration.Exceptions;

namespace SkyNeg.EntityFramework.Migration
{
    public class DatabaseManager : IDatabaseManager
    {
        private const string CoreComponent = "_core";

        private readonly IScriptProvider _scriptProvider;
        private readonly IDbContextFactory<RuntimeContext> _dbContextFactory;
        public DatabaseManager(IScriptProvider scriptProvider, IDbContextFactory<RuntimeContext> dbContextFactory)
        {
            _dbContextFactory = dbContextFactory;
            _scriptProvider = scriptProvider;
        }

        public async Task<Version?> GetComponentVersionAsync(string component, CancellationToken cancellationToken)
        {
            using (var db = await _dbContextFactory.CreateDbContextAsync(cancellationToken))
            {
                await db.Database.EnsureCreatedAsync(cancellationToken);
                var componentVersions = await db.ComponentVersions.FindAsync(component, cancellationToken);
                return componentVersions != null ? new Version(componentVersions.Version) : null;
            }
        }

        public async Task<Version?> GetVersionAsync(CancellationToken cancellationToken) => await GetComponentVersionAsync(CoreComponent, cancellationToken);

        public async Task<DatabaseUpdateResult> UpdateComponentAsync(string component, CancellationToken cancellationToken)
        {
            int scriptCounter = 0;
            Version? initialVersion = await GetComponentVersionAsync(component, cancellationToken);
            Version? currentVersion = initialVersion;
            if (currentVersion == null)
            {
                await foreach (var createScript in _scriptProvider.GetCreateScriptsAsync(cancellationToken))
                {
                    try
                    {
                        await ApplyUpdateScriptAsync(component, createScript, cancellationToken);
                    }
                    catch (Exception ex)
                    {
                        throw new DatabaseUpdateException($"Creation script failed for {component}", ex);
                    }

                    scriptCounter++;
                    currentVersion = createScript.ToVersion;
                }

                if (currentVersion == null)
                {
                    throw new DatabaseUpdateException($"Create scripts not found for {component}");
                }

                return new DatabaseUpdateResult(currentVersion, scriptCounter);
            }

            UpdateScript? updateScript;
            while ((updateScript = await _scriptProvider.GetUpdateScriptAsync(currentVersion, cancellationToken)) != null)
            {
                if (updateScript.ToVersion == null)
                {
                    throw new Exception($"Target version for update script must have a value. Update script: {updateScript}");
                }
                try
                {
                    await ApplyUpdateScriptAsync(component, updateScript, cancellationToken);
                }
                catch (Exception ex)
                {
                    throw new DatabaseUpdateException($"Can not apply update script {updateScript}", ex);
                }
                scriptCounter++;
                currentVersion = updateScript.ToVersion;
            }

            return new DatabaseUpdateResult(currentVersion, scriptCounter, initialVersion);
        }

        public async Task<DatabaseUpdateResult> UpdateAsync(CancellationToken cancellationToken) => await UpdateComponentAsync(CoreComponent, cancellationToken);


        private async Task ApplyUpdateScriptAsync(string component, UpdateScript updateScript, CancellationToken cancellationToken)
        {
            using (var db = await _dbContextFactory.CreateDbContextAsync(cancellationToken))
            {
                try
                {
                    using (var transaction = db.Database.BeginTransactionAsync(cancellationToken))
                    {
                        foreach (var sqlCommand in updateScript.SqlCommands)
                        {
                            db.Database.ExecuteSqlRaw(sqlCommand);
                        }

                        //Update component version in database
                        var componentVersion = db.Set<ComponentVersion>().FirstOrDefault(q => q.Component == component);
                        if (componentVersion == null)
                        {
                            componentVersion = new ComponentVersion();
                            componentVersion.Component = component;
                            db.Add(componentVersion);
                        }
                        componentVersion.Version = updateScript.ToVersion?.ToString() ?? string.Empty;
                        await db.SaveChangesAsync();
                        await db.Database.CommitTransactionAsync(cancellationToken);
                    }
                }
                catch
                {
                    if (db.Database.CurrentTransaction != null)
                    {
                        var rollbackTransactionId = db.Database.CurrentTransaction.TransactionId;
                        await db.Database.RollbackTransactionAsync(CancellationToken.None);
                    }
                    throw;
                }
            }
        }
    }
}
