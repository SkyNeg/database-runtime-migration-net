using Microsoft.EntityFrameworkCore;

namespace SkyNeg.EntityFramework.Migration.ScriptProviders
{
    internal class ContextScriptProvider<TContext> : IScriptProvider<TContext>
        where TContext : DbContext
    {
        private readonly IScriptProvider _scriptProvider;
        public ContextScriptProvider(IScriptProvider scriptProvider)
        {
            _scriptProvider = scriptProvider;
        }

        public IAsyncEnumerable<UpdateScript> GetCreateScriptsAsync(CancellationToken cancellationToken) => _scriptProvider.GetCreateScriptsAsync(cancellationToken);

        public Version GetMaxVersion() => _scriptProvider.GetMaxVersion();

        public Task<UpdateScript?> GetUpdateScriptAsync(Version version, CancellationToken cancellationToken) => _scriptProvider.GetUpdateScriptAsync(version, cancellationToken);
    }
}
