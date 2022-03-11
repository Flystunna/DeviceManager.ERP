using DeviceManager.Data.Models.Auditing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeviceManager.Data.Models.Dtos.Get
{
    public class GetDeviceStatusDto: FullAuditedEntity
    {
        public long Id { get; set; }
        public string Status { get; set; }
    }
}
