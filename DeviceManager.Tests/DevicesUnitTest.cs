using DeviceManager.Business.Implementations;
using DeviceManager.Business.Interfaces;
using DeviceManager.Data.Models.Entities;
using DeviceManager.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.Linq;

namespace DeviceManager.Tests
{
    [TestClass]
    public class DevicesUnitTest
    {
        [TestMethod]
        public async void TestAddAsync()
        {
            DbContextOptions<ApplicationDbContext> options;
            var builder = new DbContextOptionsBuilder<ApplicationDbContext>();
            builder.UseInMemoryDatabase("DeviceManagerDB");
            options = builder.Options;

            var newDevice = new Device 
            { 
                Id = 1, 
                Name = "Test Entity",
                DeviceStatusId = 1,
                DeviceTypeId= 1,
                Temperature = 50.01,
                CreationTime = System.DateTime.Now
            };

            using (var context = new ApplicationDbContext(options))
            {
                context.Devices.Add(newDevice);
                context.SaveChanges();
            }

            Device foundDevice;
            using (var context = new ApplicationDbContext(options))
            {
                foundDevice = await context.Devices.FirstOrDefaultAsync(x => x.Id == newDevice.Id);
            }

            Assert.IsNotNull(foundDevice);
            Assert.AreEqual(newDevice.Name, foundDevice.Name);
        }
    }
}
