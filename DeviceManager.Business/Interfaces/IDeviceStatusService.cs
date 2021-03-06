using DeviceManager.Data.Models.Dtos.Get;
using DeviceManager.Data.Models.Dtos.Post;
using DeviceManager.Data.Models.Dtos.Put;
using IPagedList;
using System.Threading.Tasks;

namespace DeviceManager.Business.Interfaces
{
    public interface IDeviceStatusService
    {
        Task<GetDeviceStatusDto> AddAsync(PostDeviceStatusDto model);
        Task<IPagedList<GetDeviceStatusDto>> GetPagedAsync(int pageNumber, int pageSize, string query);
        Task<GetDeviceStatusDto> GetAsync(long Id);
        Task<GetDeviceStatusDto> UpdateAsync(long Id, PutDeviceStatusDto model);
        Task<bool> DeleteAsync(long id);
        Task<bool> IfExists(long Id);
    }
}
