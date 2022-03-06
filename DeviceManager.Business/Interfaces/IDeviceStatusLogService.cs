using DeviceManager.Data.Models.Dtos.Get;
using DeviceManager.Data.Models.Dtos.Post;
using DeviceManager.Data.Models.Dtos.Put;
using IPagedList;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DeviceManager.Business.Interfaces
{
    public interface IDeviceStatusLogService
    {
        Task<bool> AddAsync(PostDeviceStatusLogDto model);
        Task<IPagedList<GetDeviceStatusLogDto>> GetPagedAsync(int pageNumber, int pageSize, string query);
        Task<GetDeviceStatusLogDto> GetAsync(long Id);
        Task<List<GetDeviceStatusActivityLogDto>> GetDeviceStatusActivityLog(long deviceId);
        Task<bool> UpdateAsync(long Id, PutDeviceStatusLogDto model);
        Task<bool> DeleteAsync(long id);
    }
}
