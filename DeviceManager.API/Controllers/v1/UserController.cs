using DeviceManager.Business.Implementations;
using DeviceManager.Business.Interfaces;
using DeviceManager.Data.Models.Dtos.Post;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace DeviceManager.API.Controllers.v1
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class UserController : BaseController
    {
        private readonly IUserService _userSvc;
        public UserController(IUserService userSvc)
        {
            _userSvc = userSvc;
        }

        /// <summary>
        /// Register New User.
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Route("RegisterUserAsync")]
        public async Task<IServiceResponse<bool>> AddAsync(PostNewUserDto model)
        {
            return await HandleApiOperationAsync(async () =>
            {
                var responseOBJ = await _userSvc.RegisterUserAsync(model);
                return new ServiceResponse<bool>
                {
                    Object = responseOBJ
                };
            });
        }
    }
}
