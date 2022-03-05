using System.Threading.Tasks;

namespace DeviceManager.Business.Interfaces
{
    public interface IDeviceService
    {
        Task<bool> AddAsync(Data.Models.Dtos.Post.PostDeviceDto model);
    }
}
