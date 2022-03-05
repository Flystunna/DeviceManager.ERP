using DeviceManager.Business.Implementations;
using DeviceManager.Business.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace DeviceManager.API.Controllers.v1
{
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
        [HttpGet]
        [Route("AddAsync")]
        public async Task<IServiceResponse<bool>> AddAsync(Data.Models.Dtos.Post.PostDeviceDto model)
        {
            return await HandleApiOperationAsync(async () =>
            {
                var responseOBJ = await _deviceSvc.AddAsync(model);
                return new ServiceResponse<bool>
                {
                    Object = responseOBJ
                };
            });
        }

    }
}
