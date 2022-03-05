using System.ComponentModel.DataAnnotations;

namespace DeviceManager.Data.Models.Dtos.Post
{
    public class PostDeviceTypeDto
    {
        [Required(ErrorMessage = "Device Type is required")]
        [StringLength(255, ErrorMessage = "Device Type must have max Length of 255 characters")]
        public string Type { get; set; }
    }
}
