using Microsoft.AspNetCore.Mvc.Testing;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using DeviceManager.IntegrationTests;
using Newtonsoft.Json;
using System.Net.Http;
using DeviceManager.Business.Implementations;
using System.Net.Http.Headers;
using DeviceManager.Data.Models.Dtos.Get;
using DeviceManager.Data.Models.Dtos.Post;

namespace DeviceManager.IntegrationTests.IntegrationTests
{
    public class DevicesControllerTests
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
        public async Task canAddDeviceWithAuthorization()
        {
            using var application = new WebApplicationFactory<API.Startup>();
            using var client = application.CreateClient();

            var token = await GetTokenAsync();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var body = new Data.Models.Dtos.Post.PostDeviceDto
            {
                DeviceStatusId = 1,
                DeviceTypeId = 1,
                Name = $"Integration Test Device_{Extensions.GenerateRandomNo()}",
                Temperature = Extensions.GenerateRandomTemperature()
            };
            HttpContent payload = new StringContent(JsonConvert.SerializeObject(body), Encoding.UTF8, "application/json");

            var request = await client.PostAsync("/api/v1/devices/AddAsync", payload);
            var response = await request.Content.ReadAsStringAsync();
            ServiceResponse<GetDeviceDto> result = JsonConvert.DeserializeObject<ServiceResponse<GetDeviceDto>>(response);

            Assert.Equal("200", result.Code);
            Assert.NotNull(result.Object);
        }
        [Fact]
        public async Task canNotAddDeviceWithAuthorization_Without_Device_Name()
        {
            using var application = new WebApplicationFactory<API.Startup>();
            using var client = application.CreateClient();

            var token = await GetTokenAsync();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var body = new PostDeviceDto
            {
                DeviceStatusId = 1,
                DeviceTypeId = 1,
                Temperature = Extensions.GenerateRandomTemperature()
            };
            HttpContent payload = new StringContent(JsonConvert.SerializeObject(body), Encoding.UTF8, "application/json");

            var request = await client.PostAsync("/api/v1/devices/AddAsync", payload);
            var response = await request.Content.ReadAsStringAsync();
            ServiceResponse<bool> result = JsonConvert.DeserializeObject<ServiceResponse<bool>>(response);

            Assert.Equal(400, (int)request.StatusCode);
            Assert.False(result.Object);
        }

        [Fact]
        public async Task canNotAddDeviceWithoutAuthorization()
        {
            using var application = new WebApplicationFactory<API.Startup>();
            using var client = application.CreateClient();
            var body = new PostDeviceDto
            {
                DeviceStatusId = 1,
                DeviceTypeId = 1,
                Name = $"Integration Test Device_{Extensions.GenerateRandomNo()}",
                Temperature = Extensions.GenerateRandomTemperature()
            };
            HttpContent payload = new StringContent(JsonConvert.SerializeObject(body), Encoding.UTF8, "application/json");

            var request = await client.PostAsync("/api/v1/devices/AddAsync", payload);

            Assert.Equal(401, (int)request.StatusCode);
        }


        [Fact]
        public async Task canGetSimilarDevices()
        {
            using var application = new WebApplicationFactory<API.Startup>();
            using var client = application.CreateClient();

            var token = await GetTokenAsync();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var request = await client.GetAsync("/api/v1/devices/GetSimilarDevices/1");
            var response = await request.Content.ReadAsStringAsync();
            ServiceResponse<List<GetSimilarDeviceDto>> result = JsonConvert.DeserializeObject<ServiceResponse<List<GetSimilarDeviceDto>>>(response);
           
            Assert.Equal(200, (int)request.StatusCode);
            Assert.NotEmpty(result.Object);
        }

        [Fact]
        public async Task canGetPaginatedDevices()
        {
            using var application = new WebApplicationFactory<API.Startup>();
            using var client = application.CreateClient();

            var token = await GetTokenAsync();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var request = await client.GetAsync("/api/v1/devices/GetPagedAsync/1/10");
            var response = await request.Content.ReadAsStringAsync();

            Assert.Equal(200, (int)request.StatusCode);
        }

        [Fact]
        public async Task canGetPaginatedDevicesByStatus()
        {
            using var application = new WebApplicationFactory<API.Startup>();
            using var client = application.CreateClient();

            var token = await GetTokenAsync();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var body = new PostDeviceByStatusFilterDto
            {
                pageSize = 10,
                pageNumber = 1,
                query = "",
                DeviceStatusId = 1
            };
            HttpContent payload = new StringContent(JsonConvert.SerializeObject(body), Encoding.UTF8, "application/json");

            var request = await client.PostAsync("/api/v1/devices/GetPagedDeviceByStatusAsync", payload);
            var response = await request.Content.ReadAsStringAsync();

            Assert.Equal(200, (int)request.StatusCode);
        }

        [Fact]
        public async Task canGetDeviceById()
        {
            using var application = new WebApplicationFactory<API.Startup>();
            using var client = application.CreateClient();

            var token = await GetTokenAsync();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            //Arrange Sample Object Creation
            var createBody = new PostDeviceDto
            {
                DeviceStatusId = 1,
                DeviceTypeId = 1,
                Name = $"Integration Test Device_{Extensions.GenerateRandomNo()}",
                Temperature = Extensions.GenerateRandomTemperature()
            };
            HttpContent createPayload = new StringContent(JsonConvert.SerializeObject(createBody), Encoding.UTF8, "application/json");

            var createRequest = await client.PostAsync("/api/v1/devices/AddAsync", createPayload);
            var createResponse = await createRequest.Content.ReadAsStringAsync();
            ServiceResponse<GetDeviceDto> createResult = JsonConvert.DeserializeObject<ServiceResponse<GetDeviceDto>>(createResponse);

            var request = await client.GetAsync($"/api/v1/devices/GetAsync/{createResult.Object.Id}");
            var response = await request.Content.ReadAsStringAsync();

            ServiceResponse<GetDeviceDto> result = JsonConvert.DeserializeObject<ServiceResponse<GetDeviceDto>>(response);

            Assert.Equal(200, (int)request.StatusCode);
            Assert.NotNull(result.Object);
            Assert.IsType<GetDeviceDto>(result.Object);
        }


        [Fact]
        public async Task canUpdateDevice()
        {
            //Act
            using var application = new WebApplicationFactory<API.Startup>();
            using var client = application.CreateClient();
            var token = await GetTokenAsync();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            //Arrange Sample Object Creation
            var createBody = new PostDeviceDto
            {
                DeviceStatusId = 1,
                DeviceTypeId = 1,
                Name = $"Integration Test Device_{Extensions.GenerateRandomNo()}",
                Temperature = Extensions.GenerateRandomTemperature()
            };
            HttpContent createPayload = new StringContent(JsonConvert.SerializeObject(createBody), Encoding.UTF8, "application/json");

            var createRequest = await client.PostAsync("/api/v1/devices/AddAsync", createPayload);
            var createResponse = await createRequest.Content.ReadAsStringAsync();
            ServiceResponse<GetDeviceDto> createResult = JsonConvert.DeserializeObject<ServiceResponse<GetDeviceDto>>(createResponse);

            //Arrange Sample Object update
            var updateBody = new Data.Models.Dtos.Put.PutDeviceDto
            {
                DeviceStatusId = 1,
                DeviceTypeId = 1,
                Name = $"Integration Test Device_{Extensions.GenerateRandomNo()}",
                Temperature = Extensions.GenerateRandomTemperature()
            };
            HttpContent updatePayload = new StringContent(JsonConvert.SerializeObject(updateBody), Encoding.UTF8, "application/json");

            var updateRequest = await client.PutAsync($"/api/v1/devices/UpdateAsync/{createResult.Object.Id}", updatePayload);
            var updateResponse = await updateRequest.Content.ReadAsStringAsync();
            ServiceResponse<GetDeviceDto> updateResult = JsonConvert.DeserializeObject<ServiceResponse<GetDeviceDto>>(updateResponse);

            //Assert Update Status
            Assert.NotEqual(createResult.Object.Name, updateResult.Object.Name);
            Assert.Equal(updateBody.Name, updateResult.Object.Name);
            Assert.Equal(createResult.Object.Id, updateResult.Object.Id);  
        }


        [Fact]
        public async Task canDeleteDevice()
        {
            //Act
            using var application = new WebApplicationFactory<API.Startup>();
            using var client = application.CreateClient();
            var token = await GetTokenAsync();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            //Arrange Sample Object Creation
            var createBody = new PostDeviceDto
            {
                DeviceStatusId = 1,
                DeviceTypeId = 1,
                Name = $"Integration Test Device_{Extensions.GenerateRandomNo()}",
                Temperature = Extensions.GenerateRandomTemperature()
            };
            HttpContent createPayload = new StringContent(JsonConvert.SerializeObject(createBody), Encoding.UTF8, "application/json");
            var createRequest = await client.PostAsync("/api/v1/devices/AddAsync", createPayload);
            var createResponse = await createRequest.Content.ReadAsStringAsync();
            ServiceResponse<GetDeviceDto> createResult = JsonConvert.DeserializeObject<ServiceResponse<GetDeviceDto>>(createResponse);

            //Arrange and Assert Object Deletion
            var deleteRequest = await client.DeleteAsync($"/api/v1/devices/DeleteAsync/{createResult.Object.Id}");
            var deleteResponse = await deleteRequest.Content.ReadAsStringAsync();
            ServiceResponse<bool> deleteResult = JsonConvert.DeserializeObject<ServiceResponse<bool>>(deleteResponse);
            Assert.Equal(204, int.Parse(deleteResult.Code));
            Assert.True(deleteResult.Object);


            //Assert by getting Sample Object
            var verifyRequest = await client.GetAsync($"/api/v1/devices/GetAsync/{createResult.Object.Id}");
            var verifyResponse = await verifyRequest.Content.ReadAsStringAsync();
            ServiceResponse<GetDeviceDto> verifyResult = JsonConvert.DeserializeObject<ServiceResponse<GetDeviceDto>>(verifyResponse);       
            Assert.Equal(400, int.Parse(verifyResult.Code));
            Assert.Null(verifyResult.Object);
        }
    }
}
