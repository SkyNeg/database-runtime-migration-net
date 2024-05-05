using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace SkyNeg.EntityFramework.Migration.Sample.Models
{
    [Table("MyTable")]
    public class MyTable
    {
        [Key]
        [MaxLength(100)]
        public int Id { get; set; }
        public string Value { get; set; } = string.Empty;
    }
}
