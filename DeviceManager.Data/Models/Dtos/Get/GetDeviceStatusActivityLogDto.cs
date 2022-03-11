using System;

namespace DeviceManager.Data.Models.Dtos.Get
{
    public class GetDeviceStatusActivityLogDto
    {
        public int Count { get; set; }
        public long DeviceId { get; set; }
        public long DeviceStatusId { get; set; }
        public string Date { get; set; }
        public string Month { get; set; }
        public string Year { get; set; }
        public string DeviceStatus { get; set; }
    }
}
