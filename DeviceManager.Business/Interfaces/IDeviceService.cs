using DeviceManager.Data.Models.Dtos.Get;
using DeviceManager.Data.Models.Dtos.Post;
using DeviceManager.Data.Models.Dtos.Put;
using IPagedList;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DeviceManager.Business.Interfaces
{
    public interface IDeviceService
    {
        Task<bool> AddAsync(PostDeviceDto model);
        Task<GetDeviceDto> GetAsync(long Id);
        Task<List<GetDeviceDto>> GetSimilarDevices(long deviceId);
        Task<IPagedList<GetDeviceDto>> GetPagedAsync(int pageNumber, int pageSize, string query);
        Task<bool> UpdateAsync(long Id, PutDeviceDto model);
        Task<bool> DeleteAsync(long Id);
    }
}
