using System;

namespace DeviceManager.Data.Models.Dtos.Get
{
    public class GetDeviceStatusActivityLogDto
    {
        public int Count { get; set; }
        public long DeviceId { get; set; }
        public long DeviceStatusId { get; set; }
        public DateTime Date { get; set; }
        public string DeviceStatus { get; set; }
    }
}
