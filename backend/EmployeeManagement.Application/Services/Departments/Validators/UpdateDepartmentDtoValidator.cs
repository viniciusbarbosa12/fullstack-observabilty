using EmployeeManagement.Application.Dtos.Department;
using FluentValidation;

namespace EmployeeManagement.Application.Services.Departments.Validators
{
    public class UpdateDepartmentDtoValidator : AbstractValidator<UpdateDepartmentDto>
    {
        public UpdateDepartmentDtoValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Name is required")
                .MaximumLength(100).WithMessage("Name must have at most 100 characters.");
        }
    }
}
