namespace EmployeeManagement.Application.Dtos.Employee
{
    public class UpdateEmployeeDto
    {
        public Guid Id { get; set; }
        public string FirstName { get; set; } = null!;
        public string LastName { get; set; } = null!;
        public DateTime HireDate { get; set; }
        public string? Phone { get; set; }
        public string? Address { get; set; }
        public Guid DepartmentId { get; set; }
    }

}
