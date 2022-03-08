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
    public class DeviceStatusLogControllerTests
    {
        private async Task<string> GetTokenAsync()
        {
            using var application = new WebApplicationFactory<API.Startup>();
            using var client = application.CreateClient();
            var body = new Data.Models.Dtos.Post.PostLoginDto
            {
                Password = "Admin12345.",
                Username = "admin@devicemanager.com"
            };
            HttpContent payload = new StringContent(JsonConvert.SerializeObject(body), Encoding.UTF8, "application/json");
            var response = await client.PostAsync("/api/v1/token/auth", payload);
            var result = await response.Content.ReadAsStringAsync();

            ServiceResponse<GetLoginDto> tokens = JsonConvert.DeserializeObject<ServiceResponse<GetLoginDto>>(result);
            return tokens.Object.Token;
        }

        [Fact]
        public async Task canAddDeviceStatusLogWithAuthorization()
        {
            using var application = new WebApplicationFactory<API.Startup>();
            using var client = application.CreateClient();

            var token = await GetTokenAsync();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var body = new Data.Models.Dtos.Post.PostDeviceStatusLogDto
            {
                DeviceStatusId = 1,
                DeviceId = 1
            };
            HttpContent payload = new StringContent(JsonConvert.SerializeObject(body), Encoding.UTF8, "application/json");

            var request = await client.PostAsync("/api/v1/devicestatuslog/AddAsync", payload);
            var response = await request.Content.ReadAsStringAsync();
            ServiceResponse<GetDeviceStatusLogDto> result = JsonConvert.DeserializeObject<ServiceResponse<GetDeviceStatusLogDto>>(response);

            Assert.Equal("200", result.Code);
            Assert.NotNull(result.Object);
        }

        [Fact]
        public async Task canNotAddDeviceStatusLogWithoutAuthorization()
        {
            using var application = new WebApplicationFactory<API.Startup>();
            using var client = application.CreateClient();
            var body = new Data.Models.Dtos.Post.PostDeviceStatusLogDto
            {
                DeviceStatusId = 1,
                DeviceId = 1
            };
            HttpContent payload = new StringContent(JsonConvert.SerializeObject(body), Encoding.UTF8, "application/json");

            var request = await client.PostAsync("/api/v1/devicestatuslog/AddAsync", payload);

            Assert.Equal(401, (int)request.StatusCode);
        }

        [Fact]
        public async Task canGetPaginatedDeviceStatusLog()
        {
            using var application = new WebApplicationFactory<API.Startup>();
            using var client = application.CreateClient();

            var token = await GetTokenAsync();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var request = await client.GetAsync("/api/v1/devicestatuslog/GetPagedAsync/1/10");
            var response = await request.Content.ReadAsStringAsync();

            Assert.Equal(200, (int)request.StatusCode);
        }

        [Fact]
        public async Task canGetDeviceStatusLogById()
        {
            using var application = new WebApplicationFactory<API.Startup>();
            using var client = application.CreateClient();

            var token = await GetTokenAsync();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            //Arrange Sample Object Creation
            var createBody = new Data.Models.Dtos.Post.PostDeviceStatusLogDto
            {
                DeviceStatusId = 2,
                DeviceId = 1
            };
            HttpContent createPayload = new StringContent(JsonConvert.SerializeObject(createBody), Encoding.UTF8, "application/json");

            var createRequest = await client.PostAsync("/api/v1/devicestatuslog/AddAsync", createPayload);
            var createResponse = await createRequest.Content.ReadAsStringAsync();
            ServiceResponse<GetDeviceStatusLogDto> createResult = JsonConvert.DeserializeObject<ServiceResponse<GetDeviceStatusLogDto>>(createResponse);

            var request = await client.GetAsync($"/api/v1/devicestatuslog/GetAsync/{createResult.Object.Id}");
            var response = await request.Content.ReadAsStringAsync();

            ServiceResponse<GetDeviceStatusLogDto> result = JsonConvert.DeserializeObject<ServiceResponse<GetDeviceStatusLogDto>>(response);

            Assert.Equal(200, (int)request.StatusCode);
            Assert.NotNull(result.Object);
            Assert.IsType<GetDeviceStatusLogDto>(result.Object);
        }

        [Fact]
        public async Task canUpdateDeviceStausLog()
        {
            //Act
            using var application = new WebApplicationFactory<API.Startup>();
            using var client = application.CreateClient();
            var token = await GetTokenAsync();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            //Arrange Sample Object Creation
            var createBody = new Data.Models.Dtos.Post.PostDeviceStatusLogDto
            {
                DeviceStatusId = 2,
                DeviceId = 1
            };
            HttpContent createPayload = new StringContent(JsonConvert.SerializeObject(createBody), Encoding.UTF8, "application/json");

            var createRequest = await client.PostAsync("/api/v1/devicestatuslog/AddAsync", createPayload);
            var createResponse = await createRequest.Content.ReadAsStringAsync();
            ServiceResponse<GetDeviceStatusLogDto> createResult = JsonConvert.DeserializeObject<ServiceResponse<GetDeviceStatusLogDto>>(createResponse);

            //Arrange Sample Object update
            var updateBody = new Data.Models.Dtos.Put.PutDeviceStatusLogDto
            {
                DeviceStatusId = 2,
                DeviceId = 1
            };
            HttpContent updatePayload = new StringContent(JsonConvert.SerializeObject(updateBody), Encoding.UTF8, "application/json");

            var updateRequest = await client.PutAsync($"/api/v1/devicestatuslog/UpdateAsync/{createResult.Object.Id}", updatePayload);
            var updateResponse = await updateRequest.Content.ReadAsStringAsync();
            ServiceResponse<GetDeviceStatusLogDto> updateResult = JsonConvert.DeserializeObject<ServiceResponse<GetDeviceStatusLogDto>>(updateResponse);

            //Assert Update Status
            //Assert Status ID when created is different from Status ID when updated
            Assert.NotEqual(createResult.Object.DeviceStatusId, updateResult.Object.DeviceStatusId);
            //Asert Status ID posted in update payload is same as received
            Assert.Equal(updateBody.DeviceStatusId, updateResult.Object.DeviceStatusId);
            //Assert Created object ID is the same as Updated object ID
            Assert.Equal(createResult.Object.Id, updateResult.Object.Id);
        }

        [Fact]
        public async Task canDeleteDeviceStatusLog()
        {
            //Act
            using var application = new WebApplicationFactory<API.Startup>();
            using var client = application.CreateClient();
            var token = await GetTokenAsync();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            //Arrange Sample Object Creation
            var createBody = new Data.Models.Dtos.Post.PostDeviceStatusLogDto
            {
                DeviceStatusId = 1,
                DeviceId = 1
            };
            HttpContent createPayload = new StringContent(JsonConvert.SerializeObject(createBody), Encoding.UTF8, "application/json");
            var createRequest = await client.PostAsync("/api/v1/devicestatuslog/AddAsync", createPayload);
            var createResponse = await createRequest.Content.ReadAsStringAsync();
            ServiceResponse<GetDeviceStatusLogDto> createResult = JsonConvert.DeserializeObject<ServiceResponse<GetDeviceStatusLogDto>>(createResponse);

            //Arrange and Assert Object Deletion
            var deleteRequest = await client.DeleteAsync($"/api/v1/devicestatuslog/DeleteAsync/{createResult.Object.Id}");
            var deleteResponse = await deleteRequest.Content.ReadAsStringAsync();
            ServiceResponse<bool> deleteResult = JsonConvert.DeserializeObject<ServiceResponse<bool>>(deleteResponse);
            Assert.Equal(204, int.Parse(deleteResult.Code));
            Assert.True(deleteResult.Object);


            //Assert by getting Sample Object
            var verifyRequest = await client.GetAsync($"/api/v1/devicestatuslog/GetAsync/{createResult.Object.Id}");
            var verifyResponse = await verifyRequest.Content.ReadAsStringAsync();
            ServiceResponse<GetDeviceStatusLogDto> verifyResult = JsonConvert.DeserializeObject<ServiceResponse<GetDeviceStatusLogDto>>(verifyResponse);
            Assert.Equal(400, int.Parse(verifyResult.Code));
            Assert.Null(verifyResult.Object);
        }

    }
}
