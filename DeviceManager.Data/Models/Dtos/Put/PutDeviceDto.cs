
using System.ComponentModel.DataAnnotations;

namespace DeviceManager.Data.Models.Dtos.Put
{
    public class PutDeviceDto
    {
        [Required(ErrorMessage = "Device Name is required")]
        [StringLength(255, ErrorMessage = "Device Name must have max Length of 255 characters")]
        public string Name { get; set; }
        public long? DeviceTypeId { get; set; }
        public long? DeviceStatusId { get; set; }
        public double? Temperature { get; set; }
    }
}
