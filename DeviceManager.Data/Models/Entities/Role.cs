using DeviceManager.Data.Models.Auditing;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeviceManager.Data.Models.Entities
{
    public class Role: IdentityRole<long>
    {
        public int? CreatorUserId { get; set; }
        public int? LastModifierUserId { get; set; }
        public int? DeleterUserId { get; set; }
        public DateTime CreationTime { get; set; }
        public DateTime? LastModificationTime { get; set; }
        public DateTime? DeletionTime { get; set; }
        public bool IsDeleted { get; set; }
    }
}
