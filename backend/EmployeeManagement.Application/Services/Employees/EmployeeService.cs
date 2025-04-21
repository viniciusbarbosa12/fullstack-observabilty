using EmployeeManagement.Application.Dtos.Employee;
using EmployeeManagement.Application.Service.Employees;
using EmployeeManagement.Domain.Entities;
using EmployeeManagement.Domain.Exceptions;
using EmployeeManagement.Domain.Interfaces;
using EmployeeManagement.Domain.Models;
using EmployeeManagement.Shared.Config;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Diagnostics;

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
            using var activity = Telemetry.ActivitySource.StartActivity("Create Employee");

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

            activity?.SetTag("employee.id", employee.Id.ToString());

            _logger.LogInformation("Employee {EmployeeId} created successfully.", employee.Id);

            return ToDto(employee);
        }

        public async Task<EmployeeDto> UpdateAsync(Guid id, UpdateEmployeeDto dto)
        {
            using var activity = Telemetry.ActivitySource.StartActivity("Update Employee");

            _logger.LogInformation("Starting update for employee {EmployeeId}", id);

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

            activity?.SetTag("employee.id", employee.Id.ToString());

            _logger.LogInformation("Employee {EmployeeId} updated successfully.", employee.Id);

            return ToDto(employee);
        }

        public async Task<EmployeeDto> GetByIdAsync(Guid id)
        {
            using var activity = Telemetry.ActivitySource.StartActivity("Get Employee by ID");

            _logger.LogInformation("Fetching employee {EmployeeId}", id);

            var employee = await _repository.GetByIdAsync(id)
                ?? throw new NotFoundException($"Employee with ID {id} not found.");

            activity?.SetTag("employee.id", id.ToString());

            return ToDto(employee);
        }

        public async Task<List<EmployeeDto>> GetAllAsync()
        {
            using var activity = Telemetry.ActivitySource.StartActivity("Get All Employees");

            _logger.LogInformation("Fetching all employees...");

            var employees = await _repository
                .GetAll()
                .Include(e => e.Department)
                .ToListAsync();

            activity?.SetTag("employee.count", employees.Count);

            return employees.Select(ToDto).ToList();
        }

        public async Task<PaginatedResult<EmployeeDto>> GetPagedAsync(EmployeePagedQuery query)
        {
            using var activity = Telemetry.ActivitySource.StartActivity("Get Paged Employees");

            _logger.LogInformation("Fetching paginated employee list...");

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

            activity?.SetTag("page", result.Page);
            activity?.SetTag("pageSize", result.PageSize);
            activity?.SetTag("totalItems", result.TotalItems);

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
            using var activity = Telemetry.ActivitySource.StartActivity("Delete Employee");

            _logger.LogInformation("Attempting to delete employee {EmployeeId}", id);

            var employee = await _repository.GetByIdAsync(id)
                ?? throw new NotFoundException($"Employee with ID {id} not found.");

            await _repository.DeleteAsync(employee);
            await _repository.SaveChangesAsync();

            activity?.SetTag("employee.id", id.ToString());

            _logger.LogInformation("Employee {EmployeeId} deleted successfully.", id);
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
            using var span = Telemetry.ActivitySource.StartActivity("Ensure Valid DTO");

            span?.SetTag("dto.type", typeof(T).Name);
            span?.SetTag("dto.content", dto?.ToString());

            var result = await validator.ValidateAsync(dto);

            if (!result.IsValid)
            {
                span?.SetStatus(ActivityStatusCode.Error, "Validation failed");
                span?.SetTag("validation.errors", string.Join(" | ", result.Errors.Select(e => e.ErrorMessage)));
                throw new ValidationException(result.Errors);
            }
        }

        #endregion
    }
}
