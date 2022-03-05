using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeviceManager.Data.Models.Dtos.Post
{
    public class PostLoginRefreshDto
    {
        public string Token { get; set; }
        public string RefreshToken { get; set; }
    }
}
