using FluentAssertions;
using EmployeeManagement.Application.Dtos.Employee;
using EmployeeManagement.Application.Dtos.Department;
using EmployeeManagement.Tests.Config;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Text;
using System.Net;

namespace EmployeeManagement.Tests.ControllersTest;

[Collection("Sequential")]
public class EmployeeControllerTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;

    public EmployeeControllerTests(CustomWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task Create_ShouldReturnCreatedEmployee_WhenValidDtoIsProvided()
    {
        await AuthenticateAsAdminAsync();

        var department = await GetFirstDepartmentAsync();
        var createDto = GetSampleCreateDto(department.Id);
        var content = SerializeToJsonContent(createDto);

        var response = await _client.PostAsync("/api/employee", content);

        response.StatusCode.Should().Be(HttpStatusCode.Created);

        var createdEmployee = await DeserializeResponseAsync<EmployeeDto>(response);
        createdEmployee.Should().NotBeNull();
        createdEmployee.FirstName.Should().Be(createDto.FirstName);
    }

    [Fact]
    public async Task GetAll_ShouldReturnEmployees_WhenEmployeesExist()
    {
        await AuthenticateAsAdminAsync();

        var response = await _client.GetAsync("/api/employee");

        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var employees = await DeserializeResponseAsync<List<EmployeeDto>>(response);
        employees.Should().NotBeNull();
    }

    [Fact]
    public async Task GetById_ShouldReturnEmployee_WhenEmployeeExists()
    {
        await AuthenticateAsAdminAsync();
        var employee = await CreateSampleEmployeeAsync();

        var response = await _client.GetAsync($"/api/employee/{employee.Id}");

        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var fetchedEmployee = await DeserializeResponseAsync<EmployeeDto>(response);
        fetchedEmployee.Should().NotBeNull();
        fetchedEmployee.Id.Should().Be(employee.Id);
    }

    [Fact]
    public async Task Update_ShouldReturnUpdatedEmployee_WhenValidDtoIsProvided()
    {
        await AuthenticateAsAdminAsync();
        var employee = await CreateSampleEmployeeAsync();
        var newDepartment = await GetFirstDepartmentAsync();

        employee.DepartmentId = newDepartment.Id;
        var content = SerializeToJsonContent(employee);

        var response = await _client.PutAsync($"/api/employee/{employee.Id}", content);

        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var updatedEmployee = await DeserializeResponseAsync<EmployeeDto>(response);
        updatedEmployee.Should().NotBeNull();
        updatedEmployee.FirstName.Should().Be(employee.FirstName);
    }

    [Fact]
    public async Task Delete_ShouldReturnNoContent_WhenEmployeeExists()
    {
        await AuthenticateAsAdminAsync();
        var employee = await CreateSampleEmployeeAsync();

        var response = await _client.DeleteAsync($"/api/employee/{employee.Id}");

        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }

    private async Task AuthenticateAsAdminAsync()
    {
        var token = await JwtHelper.GetAdminTokenAsync(_client);
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
    }

    private async Task<DepartmentDto> GetFirstDepartmentAsync()
    {
        var response = await _client.GetAsync("/api/department");
        var departments = await DeserializeResponseAsync<List<DepartmentDto>>(response);
        return departments.First();
    }

    private CreateEmployeeDto GetSampleCreateDto(Guid departmentId) => new()
    {
        FirstName = "John",
        LastName = "Doe",
        HireDate = DateTime.Today.AddDays(-100),
        Phone = "123456789",
        Address = "123 Main St",
        DepartmentId = departmentId
    };

    private async Task<EmployeeDto> CreateSampleEmployeeAsync()
    {
        var department = await GetFirstDepartmentAsync();
        var createDto = GetSampleCreateDto(department.Id);
        var content = SerializeToJsonContent(createDto);

        var response = await _client.PostAsync("/api/employee", content);
        return await DeserializeResponseAsync<EmployeeDto>(response);
    }

    private static StringContent SerializeToJsonContent<T>(T data)
    {
        var json = JsonConvert.SerializeObject(data);
        return new StringContent(json, Encoding.UTF8, "application/json");
    }

    private static async Task<T> DeserializeResponseAsync<T>(HttpResponseMessage response)
    {
        var content = await response.Content.ReadAsStringAsync();
        return JsonConvert.DeserializeObject<T>(content);
    }
}
