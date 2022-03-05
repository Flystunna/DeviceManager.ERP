using DeviceManager.Data.Models.Auditing;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeviceManager.Data.Models.Entities
{
    public class Device: FullAuditedEntity
    {
        [Key]
        public long Id { get; set; }
        [StringLength(255, ErrorMessage = "Device Name must have max Length of 255 characters")]
        public string Name { get; set; }
        public long? StatusId { get; set; }
        public virtual DeviceStatus Status { get; set; }
        public double Temperature { get; set; } 
    }
}
