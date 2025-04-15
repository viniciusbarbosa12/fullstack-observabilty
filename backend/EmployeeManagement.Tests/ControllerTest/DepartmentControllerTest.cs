using FluentAssertions;
using EmployeeManagement.Application.Dtos.Department;
using System.Text;
using Newtonsoft.Json;
using EmployeeManagement.Tests.Config;
using System.Net.Http.Headers;

namespace EmployeeManagement.Tests.ControllersTest
{
    [Collection("Sequential")]
    public class DepartmentControllerTests : IClassFixture<CustomWebApplicationFactory>
    {
        private readonly CustomWebApplicationFactory _factory;
        private readonly HttpClient _client;

        public DepartmentControllerTests(CustomWebApplicationFactory factory)
        {
            _factory = factory;
            _client = _factory.CreateClient();
        }


        [Fact]
        public async Task GetAll_ShouldReturnDepartments_WhenDepartmentsExist()
        {
            // Arrange
            var token = await JwtHelper.GetAdminTokenAsync(_client);
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);


            var departmentsResult = await _client.GetAsync("/api/department");
            var departmentsContent = await departmentsResult.Content.ReadAsStringAsync();
            var departmentsDto = JsonConvert.DeserializeObject<List<DepartmentDto>>(departmentsContent);


            // Assert
            departmentsResult.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);
            departmentsDto.Should().NotBeNull();
            departmentsDto.Should().HaveCountGreaterThan(0); 
        }


    }
}
