using EmployeeManagement.Application.Dtos.Employee;
using EmployeeManagement.Application.Service.Employees;
using EmployeeManagement.Domain.Entities;
using EmployeeManagement.Domain.Exceptions;
using EmployeeManagement.Domain.Interfaces;
using EmployeeManagement.Domain.Models;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace EmployeeManagement.Application.Services.Employees
{
    public class EmployeeService : IEmployeeService
    {
        private readonly IEmployeeRepository _repository;
        private readonly ILogger<EmployeeService> _logger;
        private readonly IValidator<CreateEmployeeDto> _createValidator;
        private readonly IValidator<UpdateEmployeeDto> _updateValidator;

        public EmployeeService(
            IEmployeeRepository repository,
            ILogger<EmployeeService> logger,
            IValidator<CreateEmployeeDto> createValidator,
            IValidator<UpdateEmployeeDto> updateValidator)
        {
            _repository = repository;
            _logger = logger;
            _createValidator = createValidator;
            _updateValidator = updateValidator;
        }

        public async Task<EmployeeDto> CreateAsync(CreateEmployeeDto dto)
        {
            _logger.LogInformation("Starting employee creation...");

            await EnsureValidAsync(_createValidator, dto);

            var employee = new Employee
            {
                FirstName = dto.FirstName,
                LastName = dto.LastName,
                HireDate = dto.HireDate,
                Phone = dto.Phone,
                Address = dto.Address,
                DepartmentId = dto.DepartmentId
            };

            await _repository.CreateAsync(employee);
            await _repository.SaveChangesAsync();

            _logger.LogInformation("Employee {Id} created", employee.Id);

            return ToDto(employee);
        }

        public async Task<EmployeeDto> UpdateAsync(Guid id, UpdateEmployeeDto dto)
        {
            _logger.LogInformation("Starting update for employee {Id}", id);

            var employee = await _repository.GetByIdAsync(id)
                ?? throw new NotFoundException($"Employee with ID {id} not found.");

            await EnsureValidAsync(_updateValidator, dto);

            employee.FirstName = dto.FirstName;
            employee.LastName = dto.LastName;
            employee.HireDate = dto.HireDate;
            employee.Phone = dto.Phone;
            employee.Address = dto.Address;
            employee.DepartmentId = dto.DepartmentId;

            _repository.Update(employee);
            await _repository.SaveChangesAsync();

            _logger.LogInformation("Employee {Id} updated", employee.Id);

            return ToDto(employee);
        }

        public async Task<EmployeeDto> GetByIdAsync(Guid id)
        {
            _logger.LogInformation("Fetching employee {Id}", id);
            var employee = await _repository.GetByIdAsync(id);
            if (employee == null)
                throw new NotFoundException($"Employee with ID {id} not found.");

            return ToDto(employee);
        }

        public async Task<List<EmployeeDto>> GetAllAsync()
        {
            _logger.LogInformation("Fetching all employees...");

            var employees = await _repository
                .GetAll()
                .Include(e => e.Department)
                .ToListAsync();

            return employees.Select(ToDto).ToList();
        }

        public async Task<PaginatedResult<EmployeeDto>> GetPagedAsync(EmployeePagedQuery query)
        {
            _logger.LogInformation("Fetching paged employee list...");

            var result = await _repository.GetAllPagedAsync<EmployeeFilter>(query, (dbQuery, filter) =>
            {
                if (!string.IsNullOrWhiteSpace(filter?.Name))
                {
                    var name = filter.Name.ToLower();
                    dbQuery = dbQuery.Where(e =>
                        (e.FirstName + " " + e.LastName).ToLower().Contains(name));
                }

                if (filter?.DepartmentId != null)
                    dbQuery = dbQuery.Where(e => e.DepartmentId == filter.DepartmentId);

                return dbQuery.Include(e => e.Department);
            });

            return new PaginatedResult<EmployeeDto>
            {
                Items = result.Items.Select(ToDto).ToList(),
                TotalItems = result.TotalItems,
                TotalPages = result.TotalPages,
                Page = result.Page,
                PageSize = result.PageSize
            };
        }

        public async Task DeleteAsync(Guid id)
        {
            _logger.LogInformation("Attempting to delete employee {Id}", id);

            var employee = await _repository.GetByIdAsync(id)
                ?? throw new NotFoundException($"Employee with ID {id} not found.");

            await _repository.DeleteAsync(employee);
            await _repository.SaveChangesAsync();

            _logger.LogInformation("Employee {Id} deleted", id);
        }

        #region Helpers

        private static EmployeeDto ToDto(Employee e) => new()
        {
            Id = e.Id,
            FirstName = e.FirstName,
            LastName = e.LastName,
            HireDate = e.HireDate,
            Phone = e.Phone,
            Address = e.Address,
            DepartmentName = e.Department?.Name ?? string.Empty
        };

        private static async Task EnsureValidAsync<T>(IValidator<T> validator, T dto)
        {
            var result = await validator.ValidateAsync(dto);
            if (!result.IsValid)
                throw new ValidationException(result.Errors);
        }

        #endregion
    }

}
