using DeviceManager.Data.Models.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeviceManager.Data.Models.Dtos.Get
{
    public class GetDeviceStatusActivityLogFilterDto
    {
        public long DeviceId { get; set; }
        public GroupDeviceStatusActivityLogFilter GroupByFilter { get; set; }
    }
}
