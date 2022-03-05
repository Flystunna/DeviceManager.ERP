using DeviceManager.Data.Models.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeviceManager.Domain
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser, Role, long>
    {
        public DbSet<Device> Devices { get; set; }  
        public DbSet<DeviceStatus> DeviceStatus { get; set; }  
        public DbSet<DeviceStatusLog> DeviceStatusLog { get; set; }  
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        { }
    }
}
