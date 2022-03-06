using DeviceManager.Data.Models.Auditing;
using System.Collections.Generic;

namespace DeviceManager.Data.Models.Dtos.Get
{
    public class GetDeviceDto : FullAuditedEntity
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public long? DeviceStatusId { get; set; }
        public string DeviceStatus { get; set; }
        public long? DeviceTypeId { get; set; }
        public string DeviceType { get; set; }
        public double Temperature { get; set; }
        public List<GetSimilarDeviceDto> SimilarDevices { get; set; }
        public List<GetDeviceStatusActivityLogDto> DeviceStatusActivityLog { get; set; }
    }
}
