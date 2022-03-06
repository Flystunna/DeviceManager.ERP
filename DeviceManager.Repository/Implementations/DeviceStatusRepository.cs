using DeviceManager.Data.Models.Entities;
using DeviceManager.Repository.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace DeviceManager.Repository.Implementations
{
    public class DeviceStatusRepository : Repository<DeviceStatus>, IDeviceStatusRepository
    {
        public DeviceStatusRepository(DbContext context) : base(context)
        {
        }
        public string LookUpDeviceStatusByDeviceStatusId(long deviceStatusId)
        {
            var get =  GetAsNoTracking(c=>c.Id == deviceStatusId && c.IsDeleted == false);   
            return get?.Status;
        }
    }
}
