using Moq;
using FluentAssertions;
using EmployeeManagement.Application.Services.Employees;
using EmployeeManagement.Application.Dtos.Employee;
using EmployeeManagement.Domain.Entities;
using EmployeeManagement.Domain.Interfaces;
using FluentValidation;
using Microsoft.Extensions.Logging;
using EmployeeManagement.Domain.Exceptions;
using Xunit;

namespace EmployeeManagement.Tests.ServicesTest
{
    public class EmployeeServiceTests
    {
        private readonly Mock<IEmployeeRepository> _mockRepository;
        private readonly Mock<IValidator<CreateEmployeeDto>> _mockCreateValidator;
        private readonly Mock<IValidator<UpdateEmployeeDto>> _mockUpdateValidator;
        private readonly Mock<ILogger<EmployeeService>> _mockLogger;
        private readonly EmployeeService _employeeService;

        public EmployeeServiceTests()
        {
            _mockRepository = new Mock<IEmployeeRepository>();
            _mockCreateValidator = new Mock<IValidator<CreateEmployeeDto>>();
            _mockUpdateValidator = new Mock<IValidator<UpdateEmployeeDto>>();
            _mockLogger = new Mock<ILogger<EmployeeService>>();
            _employeeService = new EmployeeService(
                _mockRepository.Object,
                _mockLogger.Object,
                _mockCreateValidator.Object,
                _mockUpdateValidator.Object
            );
        }

        [Fact]
        public async Task CreateAsync_ShouldCreateEmployee_WhenValidDtoIsProvided()
        {
            // Arrange
            var createDto = new CreateEmployeeDto
            {
                FirstName = "John",
                LastName = "Doe",
                HireDate = DateTime.UtcNow,
                Phone = "123456789",
                Address = "123 Main St",
                DepartmentId = Guid.NewGuid()
            };

            var employee = new Employee
            {
                FirstName = createDto.FirstName,
                LastName = createDto.LastName,
                HireDate = createDto.HireDate,
                Phone = createDto.Phone,
                Address = createDto.Address,
                DepartmentId = createDto.DepartmentId
            };

            _mockCreateValidator.Setup(v => v.ValidateAsync(createDto, default))
                .ReturnsAsync(new FluentValidation.Results.ValidationResult());
            _mockRepository.Setup(r => r.CreateAsync(It.IsAny<Employee>())).ReturnsAsync(employee);
            _mockRepository.Setup(r => r.SaveChangesAsync()).Returns(Task.CompletedTask);

            // Act
            var result = await _employeeService.CreateAsync(createDto);

            // Assert
            result.Should().NotBeNull();
            result.FirstName.Should().Be(createDto.FirstName);
            _mockRepository.Verify(r => r.CreateAsync(It.Is<Employee>(e => e.FirstName == createDto.FirstName)), Times.Once);
            _mockRepository.Verify(r => r.SaveChangesAsync(), Times.Once);
        }

        [Fact]
        public async Task CreateAsync_ShouldThrowValidationException_WhenDtoIsInvalid()
        {
            // Arrange
            var createDto = new CreateEmployeeDto
            {
                FirstName = string.Empty, // Invalid first name
                LastName = "Doe",
                HireDate = DateTime.UtcNow,
                Phone = "123456789",
                Address = "123 Main St",
                DepartmentId = Guid.NewGuid()
            };

            _mockCreateValidator.Setup(v => v.ValidateAsync(createDto, default))
                .ReturnsAsync(new FluentValidation.Results.ValidationResult(new[] { new FluentValidation.Results.ValidationFailure("FirstName", "First name is required.") }));

            // Act & Assert
            await Assert.ThrowsAsync<ValidationException>(() => _employeeService.CreateAsync(createDto));
        }

        [Fact]
        public async Task UpdateAsync_ShouldUpdateEmployee_WhenValidDtoIsProvided()
        {
            // Arrange
            var updateDto = new UpdateEmployeeDto
            {
                FirstName = "John",
                LastName = "Doe",
                HireDate = DateTime.UtcNow,
                Phone = "987654321",
                Address = "456 Main St",
                DepartmentId = Guid.NewGuid()
            };

            var existingEmployee = new Employee
            {
                FirstName = "Jane",
                LastName = "Doe",
                HireDate = DateTime.UtcNow.AddYears(-1),
                Phone = "123456789",
                Address = "123 Main St",
                DepartmentId = Guid.NewGuid()
            };

            _mockRepository.Setup(r => r.GetByIdAsync(existingEmployee.Id)).ReturnsAsync(existingEmployee);
            _mockUpdateValidator.Setup(v => v.ValidateAsync(updateDto, default)).ReturnsAsync(new FluentValidation.Results.ValidationResult());

            // Act
            var result = await _employeeService.UpdateAsync(existingEmployee.Id, updateDto);

            // Assert
            result.Should().NotBeNull();
            result.FirstName.Should().Be(updateDto.FirstName);
            _mockRepository.Verify(r => r.Update(It.Is<Employee>(e => e.Id == existingEmployee.Id && e.FirstName == updateDto.FirstName)), Times.Once);
            _mockRepository.Verify(r => r.SaveChangesAsync(), Times.Once);
        }

        [Fact]
        public async Task UpdateAsync_ShouldThrowNotFoundException_WhenEmployeeDoesNotExist()
        {
            // Arrange
            var employeeId = Guid.NewGuid();
            var updateDto = new UpdateEmployeeDto
            {
                FirstName = "John",
                LastName = "Doe",
                HireDate = DateTime.UtcNow,
                Phone = "987654321",
                Address = "456 Main St",
                DepartmentId = Guid.NewGuid()
            };

            _mockRepository.Setup(r => r.GetByIdAsync(employeeId)).ReturnsAsync((Employee)null);

            // Act & Assert
            await Assert.ThrowsAsync<NotFoundException>(() => _employeeService.UpdateAsync(employeeId, updateDto));
        }

        [Fact]
        public async Task DeleteAsync_ShouldDeleteEmployee_WhenEmployeeExists()
        {
            // Arrange
            var employee = new Employee
            {
                FirstName = "John",
                LastName = "Doe",
                HireDate = DateTime.UtcNow,
                Phone = "987654321",
                Address = "456 Main St",
                DepartmentId = Guid.NewGuid()
            };

            _mockRepository.Setup(r => r.GetByIdAsync(employee.Id)).ReturnsAsync(employee);
            _mockRepository.Setup(r => r.DeleteAsync(It.IsAny<Employee>())).Returns(Task.CompletedTask);
            _mockRepository.Setup(r => r.SaveChangesAsync()).Returns(Task.CompletedTask);

            // Act
            await _employeeService.DeleteAsync(employee.Id);

            // Assert
            _mockRepository.Verify(r => r.DeleteAsync(It.Is<Employee>(e => e.Id == employee.Id)), Times.Once);
            _mockRepository.Verify(r => r.SaveChangesAsync(), Times.Once);
        }

        [Fact]
        public async Task DeleteAsync_ShouldThrowNotFoundException_WhenEmployeeDoesNotExist()
        {
            // Arrange
            var employeeId = Guid.NewGuid();

            _mockRepository.Setup(r => r.GetByIdAsync(employeeId)).ReturnsAsync((Employee)null); // Employee not found

            // Act & Assert
            await Assert.ThrowsAsync<NotFoundException>(() => _employeeService.DeleteAsync(employeeId));
        }
    }
}
