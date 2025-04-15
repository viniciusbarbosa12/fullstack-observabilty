using EmployeeManagement.Application.Dtos.Employee;
using EmployeeManagement.Domain.Models;

namespace EmployeeManagement.Application.Service.Employees
{
    public interface IEmployeeService
    {
        Task<List<EmployeeDto>> GetAllAsync();
        Task<PaginatedResult<EmployeeDto>> GetPagedAsync(EmployeePagedQuery query);
        Task<EmployeeDto> GetByIdAsync(Guid id);
        Task<EmployeeDto> CreateAsync(CreateEmployeeDto dto);
        Task<EmployeeDto> UpdateAsync(Guid id, UpdateEmployeeDto dto);
        Task DeleteAsync(Guid id);

    }
}
