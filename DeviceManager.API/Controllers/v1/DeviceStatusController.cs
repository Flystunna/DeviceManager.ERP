using DeviceManager.Business.Implementations;
using DeviceManager.Business.Interfaces;
using DeviceManager.Data.Models.Dtos.Get;
using IPagedList;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace DeviceManager.API.Controllers.v1
{
    [Authorize]
    [Route("api/v1/[controller]")]
    [ApiController]
    public class DeviceStatusController : BaseController
    {
        private readonly IDeviceStatusService _deviceStatusSvc;
        public DeviceStatusController(IDeviceStatusService deviceStatusSvc)
        {
            _deviceStatusSvc = deviceStatusSvc;
        }
        /// <summary>
        /// Insert Record For New Device Status
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Route("AddAsync")]
        public async Task<IServiceResponse<GetDeviceStatusDto>> AddAsync(Data.Models.Dtos.Post.PostDeviceStatusDto model)
        {
            return await HandleApiOperationAsync(async () =>
            {
                var responseOBJ = await _deviceStatusSvc.AddAsync(model);
                return new ServiceResponse<GetDeviceStatusDto>
                {
                    Object = responseOBJ
                };
            });
        }

        /// <summary>
        /// Get Paged Record With Page Number, Page Size, Search Term
        /// </summary>
        /// <param name="pageNumber"></param>
        /// <param name="pageSize"></param>
        /// <param name="query"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("GetPagedAsync")]
        [Route("GetPagedAsync/{pageNumber}/{pageSize}")]
        [Route("GetPagedAsync/{pageNumber}/{pageSize}/{query}")]
        public async Task<IServiceResponse<IPagedList<Data.Models.Dtos.Get.GetDeviceStatusDto>>> GetPagedAsync(int pageNumber = 1, int pageSize = Core.Utils.CoreConstants.DefaultPageSize, string query = null)
        {
            return await HandleApiOperationAsync(async () => {
                var responseOBJ = await _deviceStatusSvc.GetPagedAsync(pageNumber, pageSize, query);
                return new ServiceResponse<IPagedList<Data.Models.Dtos.Get.GetDeviceStatusDto>>
                {
                    Object = responseOBJ
                };
            });
        }

        /// <summary> 
        /// Get Device Status
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("GetAsync/{id}")]
        public async Task<IServiceResponse<Data.Models.Dtos.Get.GetDeviceStatusDto>> GetAsync(long id)
        {
            return await HandleApiOperationAsync(async () => {
                var responseOBJ = await _deviceStatusSvc.GetAsync(id);
                return new ServiceResponse<Data.Models.Dtos.Get.GetDeviceStatusDto>
                {
                    Object = responseOBJ
                };
            });
        }

        /// <summary>
        /// Update Device Status
        /// </summary>
        /// <param name="id"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPut]
        [Route("UpdateAsync/{id}")]
        public async Task<IServiceResponse<GetDeviceStatusDto>> UpdateAsync(int id, Data.Models.Dtos.Put.PutDeviceStatusDto model)
        {
            return await HandleApiOperationAsync(async () => {
                var responseOBJ = await _deviceStatusSvc.UpdateAsync(id, model);
                return new ServiceResponse<GetDeviceStatusDto>
                {
                    Object = responseOBJ
                };
            });
        }

        /// <summary>
        /// Delete Device Status
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete]
        [Route("DeleteAsync/{id}")]
        public async Task<IServiceResponse<bool>> DeleteAsync(long id)
        {
            return await HandleApiOperationAsync(async () => {
                var responseOBJ = await _deviceStatusSvc.DeleteAsync(id);
                return new ServiceResponse<bool>
                {
                    Object = responseOBJ,
                    Code = responseOBJ == true ? "204" : "400"
                };
            });
        }
    }
}
