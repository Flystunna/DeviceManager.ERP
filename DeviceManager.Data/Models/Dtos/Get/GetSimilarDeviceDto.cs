using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeviceManager.Data.Models.Dtos.Get
{
    public class GetSimilarDeviceDto
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public long? DeviceStatusId { get; set; }
        public string DeviceStatus { get; set; }
        public long? DeviceTypeId { get; set; }
        public string DeviceType { get; set; }
        public double Temperature { get; set; }
    }
}
