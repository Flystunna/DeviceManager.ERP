using DeviceManager.Business.Implementations;
using DeviceManager.Business.Interfaces;
using DeviceManager.Data.Models.Dtos.Get;
using DeviceManager.Data.Models.Dtos.Post;
using DeviceManager.Data.Models.Dtos.Put;
using DeviceManager.Data.Models.Enums;
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
    public class DeviceStatusLogController : BaseController
    {
        private readonly IDeviceStatusLogService _deviceStatusLogSvc;
        public DeviceStatusLogController(IDeviceStatusLogService deviceStatusLogSvc)
        {
            _deviceStatusLogSvc = deviceStatusLogSvc;
        }

        /// <summary>
        /// Insert Record For New Device Status Log
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Route("AddAsync")]
        public async Task<IServiceResponse<GetDeviceStatusLogDto>> AddAsync(PostDeviceStatusLogDto model)
        {
            return await HandleApiOperationAsync(async () =>
            {
                var responseOBJ = await _deviceStatusLogSvc.AddAsync(model);
                return new ServiceResponse<GetDeviceStatusLogDto>
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
        public async Task<IServiceResponse<IPagedList<GetDeviceStatusLogDto>>> GetPagedAsync(int pageNumber = 1, int pageSize = Core.Utils.CoreConstants.DefaultPageSize, string query = null)
        {
            return await HandleApiOperationAsync(async () => {
                var responseOBJ = await _deviceStatusLogSvc.GetPagedAsync(pageNumber, pageSize, query);
                return new ServiceResponse<IPagedList<GetDeviceStatusLogDto>>
                {
                    Object = responseOBJ
                };
            });
        }

        /// <summary> 
        /// Get Device Status Log 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("GetAsync/{id}")]
        public async Task<IServiceResponse<GetDeviceStatusLogDto>> GetAsync(long id)
        {
            return await HandleApiOperationAsync(async () => {
                var responseOBJ = await _deviceStatusLogSvc.GetAsync(id);
                return new ServiceResponse<GetDeviceStatusLogDto>
                {
                    Object = responseOBJ
                };
            });
        }

        /// <summary> 
        /// Get All Device Status Activity Log 
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Route("GetAllDeviceStatusActivityLog")]
        public async Task<IServiceResponse<List<GetDeviceStatusActivityLogDto>>> GetAllDeviceStatusActivityLog(GetAllDeviceStatusActivityLogFilterDto model)
        {
            return await HandleApiOperationAsync(async () => {
                var responseOBJ = await _deviceStatusLogSvc.GetAllDeviceStatusActivityLog(model.DeviceStatusId);
                return new ServiceResponse<List<GetDeviceStatusActivityLogDto>>
                {
                    Object = responseOBJ
                };
            });
        }

        /// <summary> 
        /// Get Single Device Status Activity Log 
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("GetDeviceStatusActivityLog")]
        public async Task<IServiceResponse<List<GetDeviceStatusActivityLogDto>>> GetDeviceStatusActivityLog(GetDeviceStatusActivityLogFilterDto filter)
        {
            return await HandleApiOperationAsync(async () => {
                var responseOBJ = await _deviceStatusLogSvc.GetDeviceStatusActivityLog(filter.DeviceId, filter.GroupByFilter);
                return new ServiceResponse<List<GetDeviceStatusActivityLogDto>>
                {
                    Object = responseOBJ
                };
            });
        }

        /// <summary>
        /// Update Device Status Log
        /// </summary>
        /// <param name="id"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPut]
        [Route("UpdateAsync/{id}")]
        public async Task<IServiceResponse<GetDeviceStatusLogDto>> UpdateAsync(int id, PutDeviceStatusLogDto model)
        {
            return await HandleApiOperationAsync(async () => {
                var responseOBJ = await _deviceStatusLogSvc.UpdateAsync(id, model);
                return new ServiceResponse<GetDeviceStatusLogDto>
                {
                    Object = responseOBJ
                };
            });
        }

        /// <summary>
        /// Delete Device Status Log
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete]
        [Route("DeleteAsync/{id}")]
        public async Task<IServiceResponse<bool>> DeleteAsync(long id)
        {
            return await HandleApiOperationAsync(async () => {
                var responseOBJ = await _deviceStatusLogSvc.DeleteAsync(id);
                return new ServiceResponse<bool>
                {
                    Object = responseOBJ,
                    Code = responseOBJ == true ? "204" : "400"
                };
            });
        }
    }
}
