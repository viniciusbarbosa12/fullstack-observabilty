using System.Diagnostics.Metrics;

namespace EmployeeManagement.Api.Metrics
{
    public static class CustomMetrics
    {
        public static readonly Meter Meter = new("EmployeeManagementApi.Metrics", "1.0.0");

        public static readonly Counter<int> ApiRequestCounter =
            Meter.CreateCounter<int>("api_requests_total", "Total number of HTTP requests");

        public static readonly Histogram<double> ApiRequestDuration =
            Meter.CreateHistogram<double>("api_request_duration_ms", "ms", "Duration of HTTP requests in milliseconds");
    }
}
