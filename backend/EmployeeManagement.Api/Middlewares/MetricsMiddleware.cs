using System.Diagnostics;
using EmployeeManagement.Api.Metrics;

namespace EmployeeManagement.Api.Middlewares
{
    public class MetricsMiddleware
    {
        private readonly RequestDelegate _next;

        public MetricsMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var stopwatch = Stopwatch.StartNew();

            await _next(context);

            stopwatch.Stop();

            var endpoint = context.Request.Path.HasValue ? context.Request.Path.Value! : "unknown";
            var method = context.Request.Method;
            var statusCode = context.Response.StatusCode.ToString();

            // Add counter
            CustomMetrics.ApiRequestCounter.Add(1,
                KeyValuePair.Create<string, object?>("endpoint", endpoint),
                KeyValuePair.Create<string, object?>("method", method),
                KeyValuePair.Create<string, object?>("status_code", statusCode));

            // Record duration
            CustomMetrics.ApiRequestDuration.Record(stopwatch.Elapsed.TotalMilliseconds,
                KeyValuePair.Create<string, object?>("endpoint", endpoint),
                KeyValuePair.Create<string, object?>("method", method),
                KeyValuePair.Create<string, object?>("status_code", statusCode));
        }
    }
}
