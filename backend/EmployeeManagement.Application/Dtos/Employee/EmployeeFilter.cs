namespace EmployeeManagement.Application.Dtos.Employee
{
    public class EmployeeFilter
    {
        public string? Name { get; set; }
        public Guid? DepartmentId { get; set; }
    }
}
