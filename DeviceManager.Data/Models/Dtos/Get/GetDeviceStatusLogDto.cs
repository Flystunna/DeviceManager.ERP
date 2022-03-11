using DeviceManager.Data.Models.Auditing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeviceManager.Data.Models.Dtos.Get
{
    public class GetDeviceStatusLogDto : FullAuditedEntity
    {
        public long Id { get; set; }
        public long? DeviceId { get; set; }
        public string Device { get; set; }
        public long? DeviceStatusId { get; set; }
        public string DeviceStatus { get; set; }
    }
}
