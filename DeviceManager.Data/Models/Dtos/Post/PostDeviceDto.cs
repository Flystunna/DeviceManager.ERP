using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeviceManager.Data.Models.Dtos.Post
{
    public class PostDeviceDto
    {
        [Required(ErrorMessage = "Device Name is required")]
        [StringLength(255, ErrorMessage = "Device Name must have max Length of 255 characters")]
        public string Name { get; set; }
        public int StatusId { get; set; }
        public double Temperature { get; set; }
    }
}
