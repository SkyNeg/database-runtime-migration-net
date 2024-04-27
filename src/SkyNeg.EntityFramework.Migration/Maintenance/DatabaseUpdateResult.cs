namespace SkyNeg.EntityFramework.Migration
{
    public class DatabaseUpdateResult
    {
        public Version? InitialVersion { get; }
        public Version NewVersion { get; }
        public int ScriptsApplied { get; }

        public DatabaseUpdateResult(Version newVersion, int scriptsApplied, Version? initialVersion = null)
        {
            NewVersion = newVersion;
            InitialVersion = initialVersion;
            ScriptsApplied = scriptsApplied;
        }
    }
}
