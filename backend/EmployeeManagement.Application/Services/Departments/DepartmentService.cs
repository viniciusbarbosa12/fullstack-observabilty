using EmployeeManagement.Application.Dtos.Department;
using EmployeeManagement.Domain.Entities;
using EmployeeManagement.Domain.Exceptions;
using EmployeeManagement.Domain.Interfaces;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;


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
            _logger.LogInformation("Creating department...");

            await EnsureValidAsync(_createValidator, dto);

            var department = new Department { Name = dto.Name };

            await _repository.CreateAsync(department);
            await _repository.SaveChangesAsync();

            _logger.LogInformation("Department {Id} created", department.Id);

            return ToDto(department);
        }

        public async Task<DepartmentDto> UpdateAsync(Guid id, UpdateDepartmentDto dto)
        {
            _logger.LogInformation("Updating department {Id}", id);

            var department = await _repository.GetByIdAsync(id)
                ?? throw new NotFoundException($"Department with ID {id} not found.");
            
            await EnsureValidAsync(_updateValidator, dto);

            department.Name = dto.Name;
            department.MarkAsUpdated();

            _repository.Update(department);
            await _repository.SaveChangesAsync();

            return ToDto(department);
        }

        public async Task<List<DepartmentDto>> GetAllAsync()
        {
            _logger.LogInformation("Fetching all departments...");
            var list = await _repository.GetAll().ToListAsync();
            return list.Select(ToDto).ToList();
        }


        public async Task<DepartmentDto> GetByIdAsync(Guid id)
        {
            var department = await _repository.GetByIdAsync(id)
                ?? throw new NotFoundException($"Department with ID {id} not found.");

            return ToDto(department);
        }

        public async Task DeleteAsync(Guid id)
        {
            _logger.LogInformation("Deleting department {Id}", id);

            var department = await _repository.GetByIdAsync(id)
                ?? throw new NotFoundException($"Department with ID {id} not found.");

            await _repository.DeleteAsync(department);
            await _repository.SaveChangesAsync();
        }

        private static DepartmentDto ToDto(Department d) => new()
        {
            Id = d.Id,
            Name = d.Name
        };

        private static async Task EnsureValidAsync<T>(IValidator<T> validator, T dto)
        {
            var result = await validator.ValidateAsync(dto);
            if (!result.IsValid)
                throw new ValidationException(result.Errors);
        }
    }
}