using DeviceManager.Data.Models.Entities;
using DeviceManager.Domain;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeviceManager.Core.Utils
{
    public class SeedDatabase
    {
        public static void SeedDB(IApplicationBuilder app)
        {
            using (var scope = app.ApplicationServices.GetRequiredService<IServiceScopeFactory>().CreateScope())
            {
                scope.ServiceProvider.GetRequiredService<ApplicationDbContext>().Database.Migrate();
                CreateDefaultRolesAndPermissions(scope);
                CreateAdminAccount(scope);
                CreateDefaultDeviceStatus(scope);
                CreateDefaultDevice(scope);
                CreateDefaultDeviceStatusLog(scope);
            }
            Console.WriteLine("Done seeding database.");
        }
        static void CreateAdminAccount(IServiceScope serviceScope)
        {
            var userMgr = serviceScope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
            var admin = new ApplicationUser
            {
                UserName = "devicemanager",
                Email = "admin@devicemanager.com",
                EmailConfirmed = true,
                PhoneNumberConfirmed = true,
                CreationTime = DateTime.Now,
                Name = "Admin Device Manager",
                CreatorUserId = null,
                IsLoggedIn = true,
                PhoneNumber = "00000000000",
                IsDeleted = false,
                LastLoginIP = "0.0.0.0",
                LastLoginDate = DateTime.Now
            };

            if (userMgr.FindByEmailAsync(admin.Email).GetAwaiter().GetResult() is null)
            {
                var result = userMgr.CreateAsync(admin, "Admin12345.").GetAwaiter().GetResult();
                if (result.Succeeded)
                {
                    var adminRole = GetRole(serviceScope, "SuperAdmin");

                    if (adminRole != null)
                    {
                        var adminRoleResult = userMgr.AddToRoleAsync(admin, adminRole.Name).GetAwaiter().GetResult();
                    }
                }
            }
        }
        static Role GetRole(IServiceScope serviceScope, string role)
        {
            var roleMgr = serviceScope.ServiceProvider.GetRequiredService<RoleManager<Role>>();
            return roleMgr.FindByNameAsync(role).GetAwaiter().GetResult();
        }
        public static void CreateDefaultRolesAndPermissions(IServiceScope serviceScope)
        {
            var roleMgr = serviceScope.ServiceProvider.GetRequiredService<RoleManager<Role>>();
            var systemRoles = PermissionClaimsProvider.GetSystemDefaultRoles();

            if (systemRoles is null || !systemRoles.Any())
                return;

            foreach (var systemClaims in systemRoles)
            {
                var role = roleMgr.FindByNameAsync(systemClaims.Key).GetAwaiter().GetResult();
                if (role is null)
                {
                    role = new Role
                    {
                        CreationTime = DateTime.Now,
                        Name = systemClaims.Key,
                        IsDeleted = false
                    };
                    var r = roleMgr.CreateAsync(role).Result;
                }
                var oldClaims = roleMgr.GetClaimsAsync(role).GetAwaiter().GetResult();

                foreach (var claim in systemClaims.Value)
                {
                    if (!oldClaims.Any(x => x.Value.Equals(claim.Value)))
                    {
                        var r = roleMgr.AddClaimAsync(role, claim).Result;
                    }
                }
            }
        }

        static void CreateDefaultDeviceStatus(IServiceScope serviceScope)
        {
            var context = serviceScope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

            var existing_device_status = context.DeviceStatus.AsNoTracking().ToList();
            //if data exists then don't bother
            if (existing_device_status.Any())
                return;

            var device_statuses = new List<DeviceStatus>()
            {
                new DeviceStatus
                {
                    Status = "InUse",
                    CreationTime = DateTime.Now
                },
                new DeviceStatus
                {
                    Status = "Available",
                    CreationTime = DateTime.Now
                },
                new DeviceStatus
                {
                    Status = "Offline",
                    CreationTime = DateTime.Now
                }
            };

            foreach (var item in device_statuses)
            {
                context.DeviceStatus.AddRange(item);
            }
            context.SaveChanges();
        }
        static void CreateDefaultDevice(IServiceScope serviceScope)
        {
            var context = serviceScope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

            var existing_devices = context.Devices.AsNoTracking().ToList();
            //if data exists then don't bother
            if (existing_devices.Any())
                return;

            var existing_device_status = context.DeviceStatus.AsNoTracking().ToList();

            var offline = existing_device_status.FirstOrDefault(c => c.Status == "Offline")?.Id;
            var available = existing_device_status.FirstOrDefault(c => c.Status == "Available")?.Id;
            var inUse = existing_device_status.FirstOrDefault(c => c.Status == "InUse")?.Id;

            var devices = new List<Device>()
            {
                new Device
                {
                    StatusId = available,
                    Name = "Device 1",
                    Temperature = 33.99,
                    CreationTime = DateTime.Now
                },
                new Device
                {
                    Name = "Device 2",
                    Temperature = 33.99,
                    StatusId = inUse,
                    CreationTime = DateTime.Now
                },
                new Device
                {
                    Name = "Device 3",
                    Temperature = 33.99,
                    StatusId = offline,
                    CreationTime = DateTime.Now
                },
                new Device
                {
                    StatusId = available,
                    Name = "Device 4",
                    Temperature = 33.99,
                    CreationTime = DateTime.Now
                },
                new Device
                {
                    Name = "Device 5",
                    Temperature = 33.99,
                    StatusId = inUse,
                    CreationTime = DateTime.Now
                },
                new Device
                {
                    Name = "Device 6",
                    Temperature = 33.99,
                    StatusId = offline,
                    CreationTime = DateTime.Now
                },
                new Device
                {
                    Name = "Device 7",
                    Temperature = 33.99,
                    StatusId = available,
                    CreationTime = DateTime.Now
                },
                new Device
                {
                    Name = "Device 8",
                    Temperature = 33.99,
                    StatusId = inUse,
                    CreationTime = DateTime.Now
                },
                new Device
                {
                    Name = "Device 9",
                    Temperature = 33.99,
                    StatusId = offline,
                    CreationTime = DateTime.Now
                },
                new Device
                {
                    Name = "Device 10",
                    Temperature = 33.99,
                    StatusId = available,
                    CreationTime = DateTime.Now
                },
            };

            foreach (var item in devices)
            {
                if (existing_devices.FirstOrDefault(c => c.Name.ToLower() == item.Name.ToLower()) == null)
                {
                    context.Devices.Add(item);
                }
            }
            context.SaveChanges();
        }
        static void CreateDefaultDeviceStatusLog(IServiceScope serviceScope)
        {
            var context = serviceScope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            var existing_logs = context.DeviceStatusLog.AsNoTracking().Take(1).ToList();

            //if data exists then don't bother
            if (existing_logs.Any())
                return;

            var existing_devices = context.Devices.AsNoTracking().Take(5).ToList();
            var existing_device_status = context.DeviceStatus.AsNoTracking().ToList();

            var offline = existing_device_status.FirstOrDefault(c => c.Status == "Offline")?.Id;
            var available = existing_device_status.FirstOrDefault(c => c.Status == "Available")?.Id;
            var inUse = existing_device_status.FirstOrDefault(c => c.Status == "InUse")?.Id;

            var devices_status_logs = new List<DeviceStatusLog>()
            {
                new DeviceStatusLog
                {
                    StatusId = available,
                    DeviceId = existing_devices?.Count > 0 ? existing_devices[0].Id : null,
                    CreationTime = DateTime.Now
                },
                new DeviceStatusLog
                {
                    StatusId = inUse,
                    DeviceId = existing_devices?.Count > 0 ? existing_devices[0].Id : null,
                    CreationTime = DateTime.Now
                },
                new DeviceStatusLog
                {
                    StatusId = inUse,
                    DeviceId = existing_devices?.Count > 0 ? existing_devices[0].Id : null,
                    CreationTime = DateTime.Now
                },

                new DeviceStatusLog
                {
                    StatusId = offline,
                    DeviceId = existing_devices?.Count > 1 ? existing_devices[1].Id : null,
                    CreationTime = DateTime.Now
                },
                new DeviceStatusLog
                {
                    StatusId = offline,
                    DeviceId = existing_devices?.Count > 1 ? existing_devices[1].Id : null,
                    CreationTime = DateTime.Now
                },

                new DeviceStatusLog
                {
                    StatusId = inUse,
                    DeviceId = existing_devices?.Count > 2 ? existing_devices[2].Id : null,
                    CreationTime = DateTime.Now
                },
                new DeviceStatusLog
                {
                    StatusId = available,
                    DeviceId = existing_devices?.Count > 3 ? existing_devices[3].Id : null,
                    CreationTime = DateTime.Now
                },
                new DeviceStatusLog
                {
                    StatusId = inUse,
                    DeviceId = existing_devices?.Count > 4 ? existing_devices[4].Id : null,
                    CreationTime = DateTime.Now
                }
            };

            context.DeviceStatusLog.AddRange(devices_status_logs);
            context.SaveChanges();
        }
    }
}
