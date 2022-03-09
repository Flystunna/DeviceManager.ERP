using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeviceManager.Data.Models.Dtos.Post
{
    public class PostDeviceByStatusFilterDto
    {
        public int pageNumber { get; set; }
        public int pageSize { get; set; }
        public string query { get; set; }
        public long DeviceStatusId { get; set; }
    }
}
