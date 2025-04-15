using System.Diagnostics;

namespace EmployeeManagement.Infrastructure.Config
{
    public static class Observability
    {
        public static readonly ActivitySource ActivitySource = new("EmployeeManagementApi");
    }
}
