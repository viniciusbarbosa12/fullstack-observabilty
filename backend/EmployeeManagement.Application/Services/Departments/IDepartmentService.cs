using EmployeeManagement.Application.Dtos.Department;

namespace EmployeeManagement.Application.Services.Departments
{
    public interface IDepartmentService
    {
        Task<List<DepartmentDto>> GetAllAsync();
        Task<DepartmentDto> GetByIdAsync(Guid id);
        Task<DepartmentDto> CreateAsync(CreateDepartmentDto dto);
        Task<DepartmentDto> UpdateAsync(Guid id, UpdateDepartmentDto dto);
        Task DeleteAsync(Guid id);
    }
}
