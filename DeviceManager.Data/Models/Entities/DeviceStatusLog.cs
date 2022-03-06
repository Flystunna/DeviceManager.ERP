using DeviceManager.Data.Models.Auditing;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeviceManager.Data.Models.Entities
{
    public class DeviceStatusLog: FullAuditedEntity
    {
        [Key]
        public long Id { get; set; } 
        public long DeviceId { get; set; }
        public virtual Device Device { get; set; }
        public long DeviceStatusId { get; set; }
        public virtual DeviceStatus DeviceStatus { get; set; }
    }
}
