using DeviceManager.Business.Implementations;
using DeviceManager.Business.Interfaces;
using IPagedList;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace DeviceManager.API.Controllers.v1
{
    [Authorize]
    [Route("api/v1/[controller]")]
    [ApiController]
    public class DeviceTypeController : BaseController
    {
        private readonly IDeviceTypeService _DeviceTypeSvc;
        public DeviceTypeController(IDeviceTypeService DeviceTypeTypeSvc)
        {
            _DeviceTypeSvc = DeviceTypeTypeSvc; 
        }
        /// <summary>
        /// Insert Record For New Device Type
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Route("AddAsync")]
        public async Task<IServiceResponse<Data.Models.Dtos.Get.GetDeviceTypeDto>> AddAsync(Data.Models.Dtos.Post.PostDeviceTypeDto model)
        {
            return await HandleApiOperationAsync(async () =>
            {
                var responseOBJ = await _DeviceTypeSvc.AddAsync(model);
                return new ServiceResponse<Data.Models.Dtos.Get.GetDeviceTypeDto>
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
        public async Task<IServiceResponse<IPagedList<Data.Models.Dtos.Get.GetDeviceTypeDto>>> GetPagedAsync(int pageNumber = 1, int pageSize = Core.Utils.CoreConstants.DefaultPageSize, string query = null)
        {
            return await HandleApiOperationAsync(async () => {
                var responseOBJ = await _DeviceTypeSvc.GetPagedAsync(pageNumber, pageSize, query);
                return new ServiceResponse<IPagedList<Data.Models.Dtos.Get.GetDeviceTypeDto>>
                {
                    Object = responseOBJ
                };
            });
        }

        /// <summary>
        /// Get Device Type
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("GetAsync/{id}")]
        public async Task<IServiceResponse<Data.Models.Dtos.Get.GetDeviceTypeDto>> GetAsync(long id)
        {
            return await HandleApiOperationAsync(async () => {
                var responseOBJ = await _DeviceTypeSvc.GetAsync(id);
                return new ServiceResponse<Data.Models.Dtos.Get.GetDeviceTypeDto>
                {
                    Object = responseOBJ
                };
            });
        }

        /// <summary>
        /// Update Device Type
        /// </summary>
        /// <param name="id"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPut]
        [Route("UpdateAsync/{id}")]
        public async Task<IServiceResponse<Data.Models.Dtos.Get.GetDeviceTypeDto>> UpdateAsync(int id, Data.Models.Dtos.Put.PutDeviceTypeDto model)
        {
            return await HandleApiOperationAsync(async () => {
                var responseOBJ = await _DeviceTypeSvc.UpdateAsync(id, model);
                return new ServiceResponse<Data.Models.Dtos.Get.GetDeviceTypeDto>
                {
                    Object = responseOBJ
                };
            });
        }

        /// <summary>
        /// Delete Device Type
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete]
        [Route("DeleteAsync/{id}")]
        public async Task<IServiceResponse<bool>> DeleteAsync(long id)
        {
            return await HandleApiOperationAsync(async () => {
                var responseOBJ = await _DeviceTypeSvc.DeleteAsync(id);
                return new ServiceResponse<bool>
                {
                    Object = responseOBJ,
                    Code = responseOBJ ? "204" : "400"
                };
            });
        }
    }
}
