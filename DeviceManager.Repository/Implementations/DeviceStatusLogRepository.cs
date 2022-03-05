using DeviceManager.Data.Models.Entities;
using DeviceManager.Repository.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace DeviceManager.Repository.Implementations
{
    public class DeviceStatusLogRepository : Repository<DeviceStatusLog>, IDeviceStatusLogRepository
    {
        public DeviceStatusLogRepository(DbContext context) : base(context)
        {
        }
    }
}
