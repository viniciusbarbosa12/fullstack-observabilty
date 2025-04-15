using EmployeeManagement.Application.Dtos.Department;
using EmployeeManagement.Domain.Entities;
using EmployeeManagement.Domain.Exceptions;
using EmployeeManagement.Domain.Interfaces;
using EmployeeManagement.Shared.Config;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Diagnostics;

namespace EmployeeManagement.Application.Services.Departments
{
    public class DepartmentService : IDepartmentService
    {
        private readonly IDepartmentRepository _repository;
        private readonly ILogger<DepartmentService> _logger;
        private readonly IValidator<CreateDepartmentDto> _createValidator;
        private readonly IValidator<UpdateDepartmentDto> _updateValidator;

        public DepartmentService(
            IDepartmentRepository repository,
            ILogger<DepartmentService> logger,
            IValidator<CreateDepartmentDto> createValidator,
            IValidator<UpdateDepartmentDto> updateValidator)
        {
            _repository = repository;
            _logger = logger;
            _createValidator = createValidator;
            _updateValidator = updateValidator;
        }

        public async Task<DepartmentDto> CreateAsync(CreateDepartmentDto dto)
        {
            using var span = Telemetry.ActivitySource.StartActivity("Create Department");

            _logger.LogInformation("Creating department...");
            span?.AddEvent(new ActivityEvent("Start validation"));
            await EnsureValidAsync(_createValidator, dto);

            var department = new Department { Name = dto.Name };

            span?.AddEvent(new ActivityEvent("Saving department to database"));
            await _repository.CreateAsync(department);
            await _repository.SaveChangesAsync();

            span?.SetTag("department.id", department.Id.ToString());
            _logger.LogInformation("Department {DepartmentId} created", department.Id);

            return ToDto(department);
        }

        public async Task<DepartmentDto> UpdateAsync(Guid id, UpdateDepartmentDto dto)
        {
            using var span = Telemetry.ActivitySource.StartActivity("Update Department");
            _logger.LogInformation("Updating department {DepartmentId}", id);

            var department = await _repository.GetByIdAsync(id)
                ?? throw new NotFoundException($"Department with ID {id} not found.");

            span?.SetTag("department.id", id.ToString());
            span?.AddEvent(new ActivityEvent("Start validation"));
            await EnsureValidAsync(_updateValidator, dto);

            department.Name = dto.Name;
            department.MarkAsUpdated();

            span?.AddEvent(new ActivityEvent("Saving changes to database"));
            _repository.Update(department);
            await _repository.SaveChangesAsync();

            _logger.LogInformation("Department {DepartmentId} updated", id);

            return ToDto(department);
        }

        public async Task<List<DepartmentDto>> GetAllAsync()
        {
            using var span = Telemetry.ActivitySource.StartActivity("Get All Departments");
            _logger.LogInformation("Fetching all departments...");

            var list = await _repository.GetAll().ToListAsync();
            span?.SetTag("department.count", list.Count);

            return list.Select(ToDto).ToList();
        }

        public async Task<DepartmentDto> GetByIdAsync(Guid id)
        {
            using var span = Telemetry.ActivitySource.StartActivity("Get Department by ID");

            _logger.LogInformation("Fetching department {DepartmentId}", id);

            var department = await _repository.GetByIdAsync(id)
                ?? throw new NotFoundException($"Department with ID {id} not found.");

            span?.SetTag("department.id", id.ToString());

            return ToDto(department);
        }

        public async Task DeleteAsync(Guid id)
        {
            using var span = Telemetry.ActivitySource.StartActivity("Delete Department");

            _logger.LogInformation("Deleting department {DepartmentId}", id);

            var department = await _repository.GetByIdAsync(id)
                ?? throw new NotFoundException($"Department with ID {id} not found.");

            span?.SetTag("department.id", id.ToString());

            await _repository.DeleteAsync(department);
            await _repository.SaveChangesAsync();

            _logger.LogInformation("Department {DepartmentId} deleted", id);
        }

        #region Helpers

        private static DepartmentDto ToDto(Department d) => new()
        {
            Id = d.Id,
            Name = d.Name
        };

        private static async Task EnsureValidAsync<T>(IValidator<T> validator, T dto)
        {
            using var span = Telemetry.ActivitySource.StartActivity("Validate DTO");
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
