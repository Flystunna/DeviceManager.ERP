using Microsoft.AspNetCore.Mvc;

namespace DeviceManager.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HomeController : ControllerBase
    {
        [HttpGet]
        public string Get()
        {
            return "Device.Manager API is running";
        }
    }
}
