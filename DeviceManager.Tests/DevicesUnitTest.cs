using DeviceManager.Business.Implementations;
using DeviceManager.Business.Interfaces;
using DeviceManager.Data.Models.Entities;
using DeviceManager.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.Collections.Generic;
using System.Linq;

namespace DeviceManager.Tests
{
    [TestClass]
    public class DevicesUnitTest
    {
        private readonly Mock<IDeviceService> service;
        public DevicesUnitTest()
        {
            service = new Mock<IDeviceService>();
        }
        private DbContextOptions<ApplicationDbContext> Init()
        {
            var builder = new DbContextOptionsBuilder<ApplicationDbContext>();
            builder.UseInMemoryDatabase("DeviceManagerDB");
            return builder.Options;
        }
        [TestMethod]
        public void AddTest()
        {
            var options = Init();
            var newDevice = new Device 
            { 
                Id = 1, 
                Name = "Test Entity",
                DeviceStatusId = 1,
                DeviceTypeId= 1,
                Temperature = 50.01,
                CreationTime = System.DateTime.Now
            };
            Device foundDevice;

            using (var context = new ApplicationDbContext(options))
            {
                context.Devices.Add(newDevice);
                context.SaveChanges();
                foundDevice = context.Devices.FirstOrDefault(x => x.Id == newDevice.Id);
            }
            Assert.IsNotNull(foundDevice);
            Assert.AreEqual(newDevice.Name, foundDevice.Name);
        }
        public void GetAllTest()
        {
            var options = Init();
            List<Device> devices = new List<Device>();  
            using (var context = new ApplicationDbContext(options))
            {
                devices = context.Devices.ToList();
            }
            Assert.IsNotNull(devices);
        }
    }
}
