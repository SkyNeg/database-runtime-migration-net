using System.Data;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;

namespace SkyNeg.EntityFramework.Migration.ScriptProviders
{
    public class ResourceScriptProvider : IScriptProvider
    {
        private const string DefaultCreateScriptPrefix = "Data.Create";
        private const string DefaultUpdateScriptPrefix = "Data.Update";
        private const string FromVersionRegexGroup = "fromVersion";
        private const string ToVersionRegexGroup = "toVersion";
        private const string ExecutionOrderRegexGroup = "executeOrder";
        private static readonly Version DefaultVersion = new Version(1, 0);

        private readonly Assembly _assembly;
        private readonly List<ResourceScript> _createResourceScripts;
        private readonly List<ResourceScript> _updateResourceScripts;
        private readonly Regex _updateCommandRegex;
        private readonly Regex _createCommandRegex;

        public ResourceScriptProvider() : this(Assembly.GetCallingAssembly(), DefaultCreateScriptPrefix, DefaultUpdateScriptPrefix) { }

        public ResourceScriptProvider(string createScriptPrefix, string updateScriptPrefix) : this(Assembly.GetCallingAssembly(), createScriptPrefix, updateScriptPrefix) { }

        public ResourceScriptProvider(Assembly assembly, string createScriptPrefix, string updateScriptPrefix)
        {
            createScriptPrefix = string.IsNullOrEmpty(createScriptPrefix) ? DefaultCreateScriptPrefix : createScriptPrefix;
            updateScriptPrefix = string.IsNullOrEmpty(updateScriptPrefix) ? DefaultUpdateScriptPrefix : updateScriptPrefix;
            _assembly = assembly;
            var assemblyName = _assembly.GetName().Name;
            _createResourceScripts = new List<ResourceScript>();
            _updateResourceScripts = new List<ResourceScript>();

            //Assembly.Prefix.1.0.0.0_2.0.0.0_1_ScriptName.sql
            _updateCommandRegex = new Regex(Regex.Escape($"{assemblyName}.{updateScriptPrefix}.") + $@"(?<{FromVersionRegexGroup}>_\d+\._\d+(\._\d+)?(\._\d+)?)_(?<{ToVersionRegexGroup}>\d+\._\d+(\._\d+)?(\._\d+)?)\.(_(?<{ExecutionOrderRegexGroup}>\d+)_)?(.*)?\.sql");
            _updateResourceScripts = assembly.GetManifestResourceNames().Select(GetUpdateResourceScript).Where(q => q is not null).ToList()!;
            //Assembly.Prefix.1_ScriptName.sql
            _createCommandRegex = new Regex(Regex.Escape($"{assemblyName}.{createScriptPrefix}.") + $@"(?<{ExecutionOrderRegexGroup}>(\d+))(_.*)?\.sql");
            _createResourceScripts = assembly.GetManifestResourceNames().Select(GetCreateResourceScript).Where(q => q is not null).OrderBy(q => q!.ExecutionOrder).Select((q, i) => new ResourceScript(q!.ResourceName, q.TargetVersion, new Version(0, i), i)).ToList();
        }

        public async Task<UpdateScript?> GetUpdateScriptAsync(Version version, CancellationToken cancellationToken)
        {
            var maxResultVersion = _updateResourceScripts.Where(q => q.TargetVersion == version).Max(q => q.ResultVersion);
            if (maxResultVersion == null)
            {
                return null;
            }
            UpdateScript updateScript = new() { FromVersion = version, ToVersion = maxResultVersion };
            foreach (var resourceScript in _updateResourceScripts.Where(q => q.TargetVersion == version && q.ResultVersion == maxResultVersion).OrderBy(q => q.ExecutionOrder))
            {
                var sqlCommand = await GetResourceAsStringAsync(resourceScript.ResourceName, cancellationToken);
                if (!string.IsNullOrWhiteSpace(sqlCommand))
                {
                    updateScript.SqlCommands.Add(sqlCommand);
                }
            }
            return updateScript;
        }

        public async IAsyncEnumerable<UpdateScript> GetCreateScriptsAsync([EnumeratorCancellation] CancellationToken cancellationToken)
        {
            Version version = GetMaxVersion();
            UpdateScript createScript = new UpdateScript() { ToVersion = new Version() };
            int versionCounter = 0;
            for (int i = 0; i < _createResourceScripts.Count; i++)
            {
                var resourceScript = _createResourceScripts[i];
                var sqlCommand = await GetResourceAsStringAsync(resourceScript.ResourceName, cancellationToken);
                if (!string.IsNullOrWhiteSpace(sqlCommand))
                {
                    Version fromVersion = new Version(1, versionCounter);
                    versionCounter++;
                    Version toVersion = i == _createResourceScripts.Count - 1 ? GetMaxVersion() : new Version(0, versionCounter);
                    yield return new UpdateScript() { SqlCommands = { sqlCommand }, FromVersion = fromVersion, ToVersion = toVersion };
                }
            }
        }

        /// <summary>
        /// Get max version from update scripts. If no update script found return version 1.0
        /// </summary>
        /// <returns></returns>
        public Version GetMaxVersion() => _updateResourceScripts.Select(q => q.ResultVersion).Max() ?? DefaultVersion;

        private async Task<string?> GetResourceAsStringAsync(string resourceName, CancellationToken cancellationToken)
        {
            using (Stream? stream = _assembly.GetManifestResourceStream(resourceName))
            {
                if (stream == null)
                    return null;

                using (StreamReader reader = new StreamReader(stream))
                {
                    return await reader.ReadToEndAsync(cancellationToken);
                }
            }
        }

        private ResourceScript? GetUpdateResourceScript(string resourceName)
        {
            var match = _updateCommandRegex.Match(resourceName);
            if (!match.Success
                || !match.Groups.ContainsKey(FromVersionRegexGroup) || !match.Groups[FromVersionRegexGroup].Success
                || !match.Groups.ContainsKey(ToVersionRegexGroup) || !match.Groups[ToVersionRegexGroup].Success)
            {
                return null;
            }

            Version fromVersion = new Version(match.Groups[FromVersionRegexGroup].Value.Replace("_", ""));
            Version toVersion = new Version(match.Groups[ToVersionRegexGroup].Value.Replace("_", ""));

            int executionOrder = match.Groups.ContainsKey(ExecutionOrderRegexGroup) && match.Groups[ExecutionOrderRegexGroup].Success
                ? int.Parse(match.Groups[ExecutionOrderRegexGroup].Value)
                : 0;

            return new ResourceScript(resourceName, fromVersion, toVersion, executionOrder);
        }

        private ResourceScript? GetCreateResourceScript(string resourceName)
        {
            var match = _createCommandRegex.Match(resourceName);
            if (!match.Success)
            {
                return null;
            }

            Version fromVersion = new Version(0, 0);
            Version toVersion = new Version(1, 0);

            int executionOrder = match.Groups.ContainsKey(ExecutionOrderRegexGroup) && match.Groups[ExecutionOrderRegexGroup].Success
                ? int.Parse(match.Groups[ExecutionOrderRegexGroup].Value)
                : 0;

            return new ResourceScript(resourceName, fromVersion, toVersion, executionOrder);
        }
    }
}
