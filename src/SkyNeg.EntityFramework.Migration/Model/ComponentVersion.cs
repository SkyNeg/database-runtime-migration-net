using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SkyNeg.EntityFramework.Migration
{
    [Table("_ComponentVersion")]
    public class ComponentVersion
    {
        [Key]
        [MaxLength(100)]
        public string Component { get; set; } = string.Empty;
        public string Version { get; set; } = string.Empty;
    }
}
