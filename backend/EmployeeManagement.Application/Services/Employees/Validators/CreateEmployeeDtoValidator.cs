using EmployeeManagement.Application.Dtos.Employee;
using FluentValidation;

namespace EmployeeManagement.Application.Services.Employees.Validators
{
    public class CreateEmployeeDtoValidator : AbstractValidator<CreateEmployeeDto>
    {
        public CreateEmployeeDtoValidator()
        {
            RuleFor(e => e.FirstName)
                .NotEmpty().WithMessage("First name is required.")
                .MaximumLength(50);

            RuleFor(e => e.LastName)
                .NotEmpty().WithMessage("Last name is required.")
                .MaximumLength(50);

            RuleFor(e => e.HireDate)
                .LessThanOrEqualTo(DateTime.Today)
                .WithMessage("Hire date cannot be in the future.");

            RuleFor(e => e.DepartmentId)
                .NotEmpty().WithMessage("Department is required.");
        }
    }
}
