using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeviceManager.Data.Models.Dtos.Put
{
    public class PutDeviceStatusLogDto
    {
        public long? DeviceId { get; set; }
        public long? DeviceStatusId { get; set; }
    }
}
