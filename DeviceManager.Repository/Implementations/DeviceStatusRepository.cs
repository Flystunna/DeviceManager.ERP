using DeviceManager.Data.Models.Entities;
using DeviceManager.Repository.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace DeviceManager.Repository.Implementations
{
    public class DeviceStatusRepository : Repository<DeviceStatus>, IDeviceStatusRepository
    {
        public DeviceStatusRepository(DbContext context) : base(context)
        {
        }
    }
}
