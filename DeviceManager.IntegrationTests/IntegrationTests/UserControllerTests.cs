using DeviceManager.Business.Implementations;
using DeviceManager.Data.Models.Dtos.Get;
using Microsoft.AspNetCore.Mvc.Testing;
using Newtonsoft.Json;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace DeviceManager.IntegrationTests.IntegrationTests
{
    public class UserControllerTests
    {
        [Fact]
        public async Task canAddUser()
        {
            using var application = new WebApplicationFactory<API.Startup>();
            using var client = application.CreateClient();

            var body = new Data.Models.Dtos.Post.PostNewUserDto
            {
                Email = $"IntegrationTestEmail{Extensions.GenerateRandomNo()}@gmail.com",
                Name = $"IntegrationTestName{Extensions.GenerateRandomNo()}",
                Password = "Admin12345.",
                PhoneNumber = "8382983982892",
                Role = "SuperAdmin",
                Username = $"IntegrationTestEmail_{Extensions.GenerateRandomNo()}"
            };
            HttpContent payload = new StringContent(JsonConvert.SerializeObject(body), Encoding.UTF8, "application/json");

            var request = await client.PostAsync("/api/v1/user/RegisterUserAsync", payload);
            var response = await request.Content.ReadAsStringAsync();
            ServiceResponse<bool> result = JsonConvert.DeserializeObject<ServiceResponse<bool>>(response);

            Assert.Equal("200", result.Code);
            Assert.True(result.Object);
        }
    }
}
