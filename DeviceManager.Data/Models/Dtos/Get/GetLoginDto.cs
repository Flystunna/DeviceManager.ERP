using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeviceManager.Data.Models.Dtos.Get
{
    public class GetLoginDto
    {
        public string Token { get; set; }
        public string RefreshToken { get; set; }
        public int? Validity { get; set; }
        public DateTime Expires { get; set; }
        public bool IsSuperAdmin { get; set; }
        public long? UserId { get; set; }
    }
}
