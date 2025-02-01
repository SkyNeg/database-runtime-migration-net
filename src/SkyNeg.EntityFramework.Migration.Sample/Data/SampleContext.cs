using Microsoft.EntityFrameworkCore;
using SkyNeg.EntityFramework.Migration.Sample.Models;

namespace SkyNeg.EntityFramework.Migration.Sample.Data
{
    internal class SampleContext : DbContext
    {
        DbSet<MyTable> MyTables { get; set; }

        public SampleContext(DbContextOptions options) : base(options) { }
    }
}
