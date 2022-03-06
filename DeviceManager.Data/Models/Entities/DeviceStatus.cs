using DeviceManager.Data.Models.Auditing;
using System.ComponentModel.DataAnnotations;

namespace DeviceManager.Data.Models.Entities
{
    public class DeviceStatus: FullAuditedEntity
    {
        [Key]
        public long Id { get; set; }
        [StringLength(50, ErrorMessage = "Status must have max Length of 50 characters")]
        public string Status { get; set; }    
    }
}
