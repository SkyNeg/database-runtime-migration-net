using Microsoft.EntityFrameworkCore;

namespace SkyNeg.EntityFramework.Migration.Sample.Data
{
    internal class SampleContext : DbContext
    {
        public SampleContext() : base() { }

        public SampleContext(DbContextOptions options) : base(options) { }
    }
}
