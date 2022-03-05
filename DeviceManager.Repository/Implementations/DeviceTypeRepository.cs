using DeviceManager.Repository.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace DeviceManager.Repository.Implementations
{
    public class DeviceTypeRepository : Repository<Data.Models.Entities.DeviceType>, IDeviceTypeRepository
    {
        public DeviceTypeRepository(DbContext context) : base(context)
        {
        }
    }
}
