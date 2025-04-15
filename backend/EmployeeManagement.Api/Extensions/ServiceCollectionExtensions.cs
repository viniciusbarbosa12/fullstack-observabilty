using EmployeeManagement.Application.Dtos.Employee;
using EmployeeManagement.Application.Service.Auth;
using EmployeeManagement.Application.Service.Employees;
using EmployeeManagement.Application.Services.Employees.Validators;
using EmployeeManagement.Application.Services.Employees;
using FluentValidation;
using EmployeeManagement.Application.Dtos.Department;
using EmployeeManagement.Application.Services.Departments.Validators;
using EmployeeManagement.Application.Services.Departments;
using EmployeeManagement.Domain.Interfaces;
using EmployeeManagement.Infrastructure.Repositories;

namespace EmployeeManagement.Api.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {
            //Services
            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<IEmployeeService, EmployeeService>();
            services.AddScoped<IDepartmentService, DepartmentService>();

            //Validators
            services.AddScoped<IValidator<CreateEmployeeDto>, CreateEmployeeDtoValidator>();
            services.AddScoped<IValidator<UpdateEmployeeDto>, UpdateEmployeeDtoValidator>();

            services.AddScoped<IValidator<CreateDepartmentDto>, CreateDepartmentDtoValidator>();
            services.AddScoped<IValidator<UpdateDepartmentDto>, UpdateDepartmentDtoValidator>();

            //Repositories
            services.AddScoped<IEmployeeRepository, EmployeeRepository>();
            services.AddScoped<IDepartmentRepository, DepartmentRepository>();


            return services;
        }
    }
}
