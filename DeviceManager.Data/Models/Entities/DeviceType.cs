using DeviceManager.Data.Models.Auditing;
using System.ComponentModel.DataAnnotations;

namespace DeviceManager.Data.Models.Entities
{
    public class DeviceType : FullAuditedEntity
    {
        [Key]
        public long Id { get; set; }
        [StringLength(255, ErrorMessage = "Device Type must have max Length of 255 characters")]
        public string Type { get; set; }
    }
}
