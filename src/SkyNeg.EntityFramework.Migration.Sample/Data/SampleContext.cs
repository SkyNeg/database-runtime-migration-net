using Microsoft.EntityFrameworkCore;

namespace SkyNeg.EntityFramework.Migration.Sample.Data
{
    internal class SampleContext : RuntimeContext
    {
        public SampleContext(DbContextOptions options) : base(options) { }
    }
}
