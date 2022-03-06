using DeviceManager.Business.Implementations;
using DeviceManager.Business.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace DeviceManager.API.Controllers.v1
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class TokenController : BaseController
    {
        private readonly ITokenService _tokenSvc;
        public TokenController(ITokenService tokenSvc)
        {
            _tokenSvc = tokenSvc;
        }


        /// <summary>
        /// Login endpoint
        /// </summary>
        /// <param name="login"></param>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpPost]
        [Route("Auth")]
        public async Task<IServiceResponse<Data.Models.Dtos.Get.GetLoginDto>> Auth(Data.Models.Dtos.Post.PostLoginDto login)
        {
            var response = new ServiceResponse<Data.Models.Dtos.Get.GetLoginDto>();
            return await HandleApiOperationAsync(async () => {
                var authresponse = await _tokenSvc.Auth(login);
                response.Object = authresponse;
                return response;
            });
        }

        /// <summary>
        /// Logout endpoint 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Authorize]
        [HttpPut]
        [Route("Logout/{id}")]
        public async Task<IServiceResponse<bool>> Logout(int id)
        {
            return await HandleApiOperationAsync(async () => {
                var authresponse = await _tokenSvc.Logout(id);
                return new ServiceResponse<bool>
                {
                    Object = authresponse
                };
            });
        }

        /// <summary>
        /// Refresh Token endpoint
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpPost]
        [Route("RefreshToken")]
        public async Task<IServiceResponse<Data.Models.Dtos.Get.GetLoginDto>> Refresh(Data.Models.Dtos.Post.PostLoginRefreshDto model)
        {
            var response = new ServiceResponse<Data.Models.Dtos.Get.GetLoginDto>();
            return await HandleApiOperationAsync(async () => {
                var authresponse = await _tokenSvc.Refresh(model);
                response.Object = authresponse;
                return response;
            });
        }
    }
}
