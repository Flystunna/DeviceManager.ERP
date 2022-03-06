using DeviceManager.Data.Models.Auditing;
using System.ComponentModel.DataAnnotations;

namespace DeviceManager.Data.Models.Entities
{
    public class Device: FullAuditedEntity
    {
        [Key]
        public long Id { get; set; }
        [StringLength(255, ErrorMessage = "Device Name must have max Length of 255 characters")]
        public string Name { get; set; }
        public long? DeviceStatusId { get; set; }
        public virtual DeviceStatus DeviceStatus { get; set; }
        public long DeviceTypeId { get; set; }
        public virtual DeviceType DeviceType { get; set; }
        public double Temperature { get; set; } 
    }
}
