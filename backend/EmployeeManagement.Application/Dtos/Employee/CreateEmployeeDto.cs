namespace EmployeeManagement.Application.Dtos.Employee
{
    public class CreateEmployeeDto
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime HireDate { get; set; }
        public string? Phone { get; set; }
        public string? Address { get; set; }
        public Guid DepartmentId { get; set; }
    }
}
