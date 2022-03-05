using DeviceManager.Data.Models.Entities;
using DeviceManager.Data.Models.Entities.User;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace DeviceManager.Domain
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser, Role, long>
    {
        public DbSet<Device> Devices { get; set; }  
        public DbSet<DeviceStatus> DeviceStatus { get; set; }  
        public DbSet<DeviceType> DeviceType { get; set; }  
        public DbSet<DeviceStatusLog> DeviceStatusLog { get; set; }  
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        { }
    }
}
