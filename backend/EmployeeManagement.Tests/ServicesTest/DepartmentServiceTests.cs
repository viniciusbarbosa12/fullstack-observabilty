using Moq;
using FluentAssertions;
using EmployeeManagement.Application.Services.Departments;
using EmployeeManagement.Application.Dtos.Department;
using EmployeeManagement.Domain.Entities;
using EmployeeManagement.Domain.Interfaces;
using FluentValidation;
using Microsoft.Extensions.Logging;
using EmployeeManagement.Domain.Exceptions;
using MockQueryable.Moq;

namespace EmployeeManagement.Tests.ServicesTest
{
    public class DepartmentServiceTests
    {
        private readonly Mock<IDepartmentRepository> _mockRepository;
        private readonly Mock<IValidator<CreateDepartmentDto>> _mockCreateValidator;
        private readonly Mock<IValidator<UpdateDepartmentDto>> _mockUpdateValidator;
        private readonly Mock<ILogger<DepartmentService>> _mockLogger;
        private readonly DepartmentService _departmentService;

        public DepartmentServiceTests()
        {
            _mockRepository = new Mock<IDepartmentRepository>();
            _mockCreateValidator = new Mock<IValidator<CreateDepartmentDto>>();
            _mockUpdateValidator = new Mock<IValidator<UpdateDepartmentDto>>();
            _mockLogger = new Mock<ILogger<DepartmentService>>();
            _departmentService = new DepartmentService(
                _mockRepository.Object,
                _mockLogger.Object,
                _mockCreateValidator.Object,
                _mockUpdateValidator.Object
            );
        }

        [Fact]
        public async Task CreateAsync_ShouldCreateDepartment_WhenValidDtoIsProvided()
        {
            // Arrange
            var createDto = new CreateDepartmentDto
            {
                Name = "HR"
            };

            var department = new Department
            {
                Name = createDto.Name
            };

            _mockCreateValidator.Setup(v => v.ValidateAsync(createDto, default))
                .ReturnsAsync(new FluentValidation.Results.ValidationResult());
            _mockRepository.Setup(r => r.CreateAsync(It.IsAny<Department>())).ReturnsAsync(department);
            _mockRepository.Setup(r => r.SaveChangesAsync()).Returns(Task.CompletedTask);

            // Act
            var result = await _departmentService.CreateAsync(createDto);

            // Assert
            result.Should().NotBeNull();
            result.Name.Should().Be(createDto.Name);
            _mockRepository.Verify(r => r.CreateAsync(It.Is<Department>(d => d.Name == createDto.Name)), Times.Once);
            _mockRepository.Verify(r => r.SaveChangesAsync(), Times.Once);
        }

        [Fact]
        public async Task UpdateAsync_ShouldUpdateDepartment_WhenValidDtoIsProvided()
        {
            // Arrange
            var departmentId = Guid.NewGuid();
            var updateDto = new UpdateDepartmentDto
            {
                Name = "Marketing"
            };

            var existingDepartment = new Department
            {
                Name = "Sales"
            };

            _mockRepository.Setup(r => r.GetByIdAsync(departmentId)).ReturnsAsync(existingDepartment);
            _mockUpdateValidator.Setup(v => v.ValidateAsync(updateDto, default)).ReturnsAsync(new FluentValidation.Results.ValidationResult());

            // Act
            var result = await _departmentService.UpdateAsync(departmentId, updateDto);

            // Assert
            result.Should().NotBeNull();
            result.Name.Should().Be(updateDto.Name);
            _mockRepository.Verify(r => r.Update(It.Is<Department>(d => d.Name == updateDto.Name)), Times.Once);
            _mockRepository.Verify(r => r.SaveChangesAsync(), Times.Once);
        }

        [Fact]
        public async Task UpdateAsync_ShouldThrowNotFoundException_WhenDepartmentDoesNotExist()
        {
            // Arrange
            var departmentId = Guid.NewGuid();
            var updateDto = new UpdateDepartmentDto
            {
                Name = "Marketing"
            };

            _mockRepository.Setup(r => r.GetByIdAsync(departmentId)).ReturnsAsync((Department)null);

            // Act & Assert
            await Assert.ThrowsAsync<NotFoundException>(() => _departmentService.UpdateAsync(departmentId, updateDto));
        }


        [Fact]
        public async Task GetAllAsync_ShouldReturnListOfDepartments()
        {
            // Arrange
            var departments = new List<Department>
            {
                new Department { Name = "HR" },
                new Department { Name = "IT" }
            };

            var mockDbSet = departments.AsQueryable().BuildMockDbSet();

            _mockRepository.Setup(r => r.GetAll()).Returns(mockDbSet.Object);
            // Act
            var result = await _departmentService.GetAllAsync();

            // Assert
            result.Should().HaveCount(2);
            result[0].Name.Should().Be(departments[0].Name);
            result[1].Name.Should().Be(departments[1].Name);
        }

        [Fact]
        public async Task GetByIdAsync_ShouldReturnDepartment_WhenDepartmentExists()
        {
            // Arrange
            var departmentId = Guid.NewGuid();
            var department = new Department
            {
                Name = "HR"
            };

            _mockRepository.Setup(r => r.GetByIdAsync(departmentId)).ReturnsAsync(department);

            // Act
            var result = await _departmentService.GetByIdAsync(departmentId);

            // Assert
            result.Should().NotBeNull();
            result.Name.Should().Be(department.Name);
        }

        [Fact]
        public async Task GetByIdAsync_ShouldThrowNotFoundException_WhenDepartmentDoesNotExist()
        {
            // Arrange
            var departmentId = Guid.NewGuid();

            _mockRepository.Setup(r => r.GetByIdAsync(departmentId)).ReturnsAsync((Department)null);

            // Act & Assert
            await Assert.ThrowsAsync<NotFoundException>(() => _departmentService.GetByIdAsync(departmentId));
        }

        [Fact]
        public async Task DeleteAsync_ShouldDeleteDepartment_WhenDepartmentExists()
        {
            // Arrange
            var department = new Department { Name = "HR" };

            _mockRepository.Setup(r => r.GetByIdAsync(department.Id)).ReturnsAsync(department);
            _mockRepository.Setup(r => r.DeleteAsync(It.IsAny<Department>())).Returns(Task.CompletedTask);
            _mockRepository.Setup(r => r.SaveChangesAsync()).Returns(Task.CompletedTask);

            // Act
            await _departmentService.DeleteAsync(department.Id);

            // Assert
            _mockRepository.Verify(r => r.DeleteAsync(It.Is<Department>(d => d.Id == department.Id)), Times.Once);
            _mockRepository.Verify(r => r.SaveChangesAsync(), Times.Once);
        }


        [Fact]
        public async Task DeleteAsync_ShouldThrowNotFoundException_WhenDepartmentDoesNotExist()
        {
            // Arrange
            var departmentId = Guid.NewGuid();

            _mockRepository.Setup(r => r.GetByIdAsync(departmentId)).ReturnsAsync((Department)null);

            // Act & Assert
            await Assert.ThrowsAsync<NotFoundException>(() => _departmentService.DeleteAsync(departmentId));
        }


    }


}