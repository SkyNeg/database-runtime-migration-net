namespace SkyNeg.EntityFramework.Migration
{
    internal class ResourceScript
    {
        public Version TargetVersion { get; set; }
        public Version ResultVersion { get; set; }
        public int ExecutionOrder { get; set; }
        public string ResourceName { get; set; }

        public ResourceScript(string resourceName, Version targetVersion, Version resultVersion, int executionOrder)
        {
            ResourceName = resourceName;
            TargetVersion = targetVersion;
            ResultVersion = resultVersion;
            ExecutionOrder = executionOrder;
        }
    }
}
