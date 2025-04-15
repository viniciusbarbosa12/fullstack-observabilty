using EmployeeManagement.Domain.Entities;
using EmployeeManagement.Domain.Interfaces.Base;

namespace EmployeeManagement.Domain.Interfaces
{
    public interface IEmployeeRepository : IRepository<Employee>
    {
    }
}
