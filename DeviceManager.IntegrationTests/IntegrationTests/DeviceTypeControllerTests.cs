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
    public class DeviceTypeControllerTests
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
        public async Task canAddDeviceTypeWithAuthorization()
        {
            using var application = new WebApplicationFactory<API.Startup>();
            using var client = application.CreateClient();

            var token = await GetTokenAsync();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var body = new Data.Models.Dtos.Post.PostDeviceTypeDto
            {
                Type = $"Integration Test Device_Type_{Extensions.GenerateRandomNo()}"
            };
            HttpContent payload = new StringContent(JsonConvert.SerializeObject(body), Encoding.UTF8, "application/json");

            var request = await client.PostAsync("/api/v1/devicetype/AddAsync", payload);
            var response = await request.Content.ReadAsStringAsync();
            ServiceResponse<GetDeviceTypeDto> result = JsonConvert.DeserializeObject<ServiceResponse<GetDeviceTypeDto>>(response);

            Assert.Equal("200", result.Code);
            Assert.NotNull(result.Object);
        }

        [Fact]
        public async Task canNotAddDeviceTypeWithoutAuthorization()
        {
            using var application = new WebApplicationFactory<API.Startup>();
            using var client = application.CreateClient();
            var body = new Data.Models.Dtos.Post.PostDeviceTypeDto
            {
                Type = $"Integration Test Device_Type_{Extensions.GenerateRandomNo()}"
            };
            HttpContent payload = new StringContent(JsonConvert.SerializeObject(body), Encoding.UTF8, "application/json");

            var request = await client.PostAsync("/api/v1/devicetype/AddAsync", payload);

            Assert.Equal(401, (int)request.StatusCode);
        }

        [Fact]
        public async Task canGetPaginatedDeviceType()
        {
            using var application = new WebApplicationFactory<API.Startup>();
            using var client = application.CreateClient();

            var token = await GetTokenAsync();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var request = await client.GetAsync("/api/v1/devicetype/GetPagedAsync/1/10");
            var response = await request.Content.ReadAsStringAsync();

            Assert.Equal(200, (int)request.StatusCode);
        }

        [Fact]
        public async Task canGetDeviceTypeById()
        {
            using var application = new WebApplicationFactory<API.Startup>();
            using var client = application.CreateClient();

            var token = await GetTokenAsync();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            //Arrange Sample Object Creation
            var createBody = new Data.Models.Dtos.Post.PostDeviceTypeDto
            {
                Type = $"Integration Test Device_Type_{Extensions.GenerateRandomNo()}"
            };
            HttpContent createPayload = new StringContent(JsonConvert.SerializeObject(createBody), Encoding.UTF8, "application/json");

            var createRequest = await client.PostAsync("/api/v1/devicetype/AddAsync", createPayload);
            var createResponse = await createRequest.Content.ReadAsStringAsync();
            ServiceResponse<GetDeviceTypeDto> createResult = JsonConvert.DeserializeObject<ServiceResponse<GetDeviceTypeDto>>(createResponse);

            var request = await client.GetAsync($"/api/v1/devicetype/GetAsync/{createResult.Object.Id}");
            var response = await request.Content.ReadAsStringAsync();

            ServiceResponse<GetDeviceTypeDto> result = JsonConvert.DeserializeObject<ServiceResponse<GetDeviceTypeDto>>(response);

            Assert.Equal(200, (int)request.StatusCode);
            Assert.NotNull(result.Object);
            Assert.IsType<GetDeviceTypeDto>(result.Object);
        }

        [Fact]
        public async Task canUpdateDeviceType()
        {
            //Act
            using var application = new WebApplicationFactory<API.Startup>();
            using var client = application.CreateClient();
            var token = await GetTokenAsync();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            //Arrange Sample Object Creation
            var createBody = new Data.Models.Dtos.Post.PostDeviceTypeDto
            {
                Type = $"Integration Test Device_Type_{Extensions.GenerateRandomNo()}"
            };
            HttpContent createPayload = new StringContent(JsonConvert.SerializeObject(createBody), Encoding.UTF8, "application/json");

            var createRequest = await client.PostAsync("/api/v1/devicetype/AddAsync", createPayload);
            var createResponse = await createRequest.Content.ReadAsStringAsync();
            ServiceResponse<GetDeviceTypeDto> createResult = JsonConvert.DeserializeObject<ServiceResponse<GetDeviceTypeDto>>(createResponse);

            //Arrange Sample Object update
            var updateBody = new Data.Models.Dtos.Put.PutDeviceTypeDto
            {
                Type = $"Integration Test Device_Type_{Extensions.GenerateRandomNo()}"
            };
            HttpContent updatePayload = new StringContent(JsonConvert.SerializeObject(updateBody), Encoding.UTF8, "application/json");

            var updateRequest = await client.PutAsync($"/api/v1/devicetype/UpdateAsync/{createResult.Object.Id}", updatePayload);
            var updateResponse = await updateRequest.Content.ReadAsStringAsync();
            ServiceResponse<GetDeviceTypeDto> updateResult = JsonConvert.DeserializeObject<ServiceResponse<GetDeviceTypeDto>>(updateResponse);

            //Assert Update Type
            //Assert Type when created is different from type when updated
            Assert.NotEqual(createResult.Object.Type, updateResult.Object.Type);
            //Asert Type posted in update payload is same as received
            Assert.Equal(updateBody.Type, updateResult.Object.Type);
            //Assert Created object ID is the same as Updated object ID
            Assert.Equal(createResult.Object.Id, updateResult.Object.Id);
        }

        [Fact]
        public async Task canDeleteDeviceType()
        {
            //Act
            using var application = new WebApplicationFactory<API.Startup>();
            using var client = application.CreateClient();
            var token = await GetTokenAsync();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            //Arrange Sample Object Creation
            var createBody = new Data.Models.Dtos.Post.PostDeviceTypeDto
            {
                Type = $"Integration Test Device_Type_{Extensions.GenerateRandomNo()}"
            };
            HttpContent createPayload = new StringContent(JsonConvert.SerializeObject(createBody), Encoding.UTF8, "application/json");
            var createRequest = await client.PostAsync("/api/v1/devicetype/AddAsync", createPayload);
            var createResponse = await createRequest.Content.ReadAsStringAsync();
            ServiceResponse<GetDeviceTypeDto> createResult = JsonConvert.DeserializeObject<ServiceResponse<GetDeviceTypeDto>>(createResponse);

            //Arrange and Assert Object Deletion
            var deleteRequest = await client.DeleteAsync($"/api/v1/devicetype/DeleteAsync/{createResult.Object.Id}");
            var deleteResponse = await deleteRequest.Content.ReadAsStringAsync();
            ServiceResponse<bool> deleteResult = JsonConvert.DeserializeObject<ServiceResponse<bool>>(deleteResponse);
            Assert.Equal(204, int.Parse(deleteResult.Code));
            Assert.True(deleteResult.Object);


            //Assert by getting Sample Object
            var verifyRequest = await client.GetAsync($"/api/v1/devicetype/GetAsync/{createResult.Object.Id}");
            var verifyResponse = await verifyRequest.Content.ReadAsStringAsync();
            ServiceResponse<GetDeviceTypeDto> verifyResult = JsonConvert.DeserializeObject<ServiceResponse<GetDeviceTypeDto>>(verifyResponse);
            Assert.Equal(400, int.Parse(verifyResult.Code));
            Assert.Null(verifyResult.Object);
        }

    }
}
