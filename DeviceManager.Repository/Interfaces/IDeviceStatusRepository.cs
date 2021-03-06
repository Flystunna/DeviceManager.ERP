using DeviceManager.Data.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeviceManager.Repository.Interfaces
{
    public interface IDeviceStatusRepository : IRepository<DeviceStatus>
    {
        string LookUpDeviceStatusByDeviceStatusId(long deviceStatusId);
    }
}
