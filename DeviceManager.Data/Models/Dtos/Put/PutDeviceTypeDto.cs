using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeviceManager.Data.Models.Dtos.Put
{
    public class PutDeviceTypeDto
    {
        [Required(ErrorMessage = "Device Type is required")]
        [StringLength(255, ErrorMessage = "Device Type must have max Length of 255 characters")]
        public string Type { get; set; }
    }
}
