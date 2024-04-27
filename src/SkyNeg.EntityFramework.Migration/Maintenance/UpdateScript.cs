namespace SkyNeg.EntityFramework.Migration
{
    /// <summary>
    /// An update script that consist of one or few commands that can be executed within a single transaction.
    /// </summary>
    public class UpdateScript
    {
        public Version? FromVersion { get; set; }
        public Version ToVersion { get; set; } = new();
        public List<string> SqlCommands { get; set; } = new List<string>();

        public override string ToString()
        {
            return $"{FromVersion}-{ToVersion}";
        }
    }
}
