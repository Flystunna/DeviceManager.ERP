using DeviceManager.Business.Implementations;
using DeviceManager.Business.Interfaces;
using DeviceManager.Data.Models.Dtos.Get;
using DeviceManager.Data.Models.Dtos.Post;
using IPagedList;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DeviceManager.API.Controllers.v1
{
    [Authorize]
    [Route("api/v1/[controller]")]
    [ApiController]
    public class DevicesController : BaseController
    {
        private readonly IDeviceService _deviceSvc;
        public DevicesController(IDeviceService deviceSvc)
        {
            _deviceSvc = deviceSvc;
        }
        /// <summary>
        /// Insert Record For New Device
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Route("AddAsync")]
        public async Task<IServiceResponse<GetDeviceDto>> AddAsync(Data.Models.Dtos.Post.PostDeviceDto model)
        {
            return await HandleApiOperationAsync(async () =>
            {
                var responseOBJ = await _deviceSvc.AddAsync(model);
                return new ServiceResponse<GetDeviceDto>
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
        public async Task<IServiceResponse<IPagedList<GetDeviceDto>>> GetPagedAsync(int pageNumber = 1, int pageSize = Core.Utils.CoreConstants.DefaultPageSize, string query = null)
        {
            return await HandleApiOperationAsync(async () => {
                var responseOBJ = await _deviceSvc.GetPagedAsync(pageNumber, pageSize, query);
                return new ServiceResponse<IPagedList<GetDeviceDto>>
                {
                    Object = responseOBJ
                };
            });
        }

        /// <summary>
        /// Get Device By Status Paged Record With Page Number, Page Size, Search Term
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Route("GetPagedDeviceByStatusAsync")]
        public async Task<IServiceResponse<IPagedList<GetDeviceDto>>> GetPagedDeviceByStatusAsync(PostDeviceByStatusFilterDto model)
        {
            return await HandleApiOperationAsync(async () => {
                var responseOBJ = await _deviceSvc.GetPagedDeviceByStatusAsync(model);
                return new ServiceResponse<IPagedList<GetDeviceDto>>
                {
                    Object = responseOBJ
                };
            });
        }

        /// <summary>
        /// Get Device
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("GetAsync/{id}")]
        public async Task<IServiceResponse<GetDeviceDto>> GetAsync(long id)
        {
            return await HandleApiOperationAsync(async () => {
                var responseOBJ = await _deviceSvc.GetAsync(id);
                return new ServiceResponse<GetDeviceDto>
                {
                    Object = responseOBJ
                };
            });
        }

        /// <summary>
        /// Get Similar Devices 
        /// </summary>
        /// <param name="deviceId"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("GetSimilarDevices/{deviceId}")]
        public async Task<IServiceResponse<List<GetSimilarDeviceDto>>> GetSimilarDevices(long deviceId)
        {
            return await HandleApiOperationAsync(async () => {
                var responseOBJ = await _deviceSvc.GetSimilarDevices(deviceId);
                return new ServiceResponse<List<GetSimilarDeviceDto>>
                {
                    Object = responseOBJ
                };
            });
        }

        /// <summary>
        /// Update Device
        /// </summary>
        /// <param name="id"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPut]
        [Route("UpdateAsync/{id}")]
        public async Task<IServiceResponse<GetDeviceDto>> UpdateAsync(int id, Data.Models.Dtos.Put.PutDeviceDto model)
        {
            return await HandleApiOperationAsync(async () => {
                var responseOBJ = await _deviceSvc.UpdateAsync(id, model);
                return new ServiceResponse<GetDeviceDto>
                {
                    Object = responseOBJ
                };
            });
        }

        /// <summary>
        /// Delete Device
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete]
        [Route("DeleteAsync/{id}")]
        public async Task<IServiceResponse<bool>> DeleteAsync(long id)
        {
            return await HandleApiOperationAsync(async () => {
                var responseOBJ = await _deviceSvc.DeleteAsync(id);
                return new ServiceResponse<bool>
                {
                    Object = responseOBJ,
                    Code = responseOBJ == true ? "204" : "400"
                };
            });
        }
    }
}
