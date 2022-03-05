using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeviceManager.Business.Interfaces
{
    public interface ITokenService
    {
        Task<Data.Models.Dtos.Get.GetLoginDto> Auth(Data.Models.Dtos.Post.PostLoginDto login);
        Task<Data.Models.Dtos.Get.GetLoginDto> Refresh(Data.Models.Dtos.Post.PostLoginRefreshDto model);
        Task<bool> Logout(long? id);
    }
}
