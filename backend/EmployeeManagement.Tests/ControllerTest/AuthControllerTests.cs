using EmployeeManagement.Application.Dtos.Employee;
using EmployeeManagement.Tests.Config;
using Newtonsoft.Json;
using System.Net;
using System.Text;
using FluentAssertions;

namespace EmployeeManagement.Tests.ControllersTest
{
    [Collection("Sequential")]
    public class AuthControllerTests : IClassFixture<CustomWebApplicationFactory>
    {
        private readonly HttpClient _client;

        public AuthControllerTests(CustomWebApplicationFactory factory)
        {
            _client = factory.CreateClient();
        }

        [Fact]
        public async Task ShouldReturnToken_WhenLoginIsSuccessful()
        {
            var credentials = GetValidCredentials();

            await RegisterUserAsync(credentials);

            var response = await _client.PostAsync("/api/auth/login", SerializeToJsonContent(credentials));

            response.StatusCode.Should().Be(HttpStatusCode.OK);

            var token = await ExtractTokenAsync(response);
            token.Should().NotBeNullOrEmpty();
        }

        [Fact]
        public async Task ShouldReturnEmptyToken_WhenLoginFails()
        {
            var invalidCredentials = new
            {
                Password = "WrongPassword",
                UserName = "admin@admin.com",
                Email = "admin@admin.com",
            };

            var response = await _client.PostAsync("/api/auth/login", SerializeToJsonContent(invalidCredentials));

            response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);

            var token = await ExtractTokenAsync(response);
            token.Should().BeNullOrEmpty();
        }

        private static object GetValidCredentials() => new
        {
            Password = "Admin@123",
            UserName = "admin@admin.com",
            Email = "admin@admin.com",
        };

        private async Task RegisterUserAsync(object credentials)
        {
            await _client.PostAsync("/api/auth/register", SerializeToJsonContent(credentials));
        }

        private static StringContent SerializeToJsonContent(object data)
        {
            var json = JsonConvert.SerializeObject(data);
            return new StringContent(json, Encoding.UTF8, "application/json");
        }

        private static async Task<string> ExtractTokenAsync(HttpResponseMessage response)
        {
            var content = await response.Content.ReadAsStringAsync();
            var result = JsonConvert.DeserializeObject<TokenResponse>(content);
            return result?.Token ?? string.Empty;
        }
    }

}