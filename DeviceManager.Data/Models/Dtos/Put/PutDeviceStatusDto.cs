using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeviceManager.Data.Models.Dtos.Put
{
    public class PutDeviceStatusDto
    {
        [StringLength(50, ErrorMessage = "Status must have max Length of 50 characters")]
        public string Status { get; set; }
    }
}
