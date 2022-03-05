using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeviceManager.Data.Models.Dtos.Post
{
    public class PostDeviceStatusLogDto
    {
        public long? DeviceId { get; set; }
        public long? StatusId { get; set; }
    }
}
