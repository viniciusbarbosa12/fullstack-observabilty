using System.Diagnostics;

namespace EmployeeManagement.Shared.Config
{
    public static class Telemetry
    {
        public static readonly ActivitySource ActivitySource = new("EmployeeManagementApi");

    }
}
