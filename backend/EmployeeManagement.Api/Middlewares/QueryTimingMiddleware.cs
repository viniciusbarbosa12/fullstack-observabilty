using System.Diagnostics;
using EmployeeManagement.Api.Diagnostics;

namespace EmployeeManagement.Api.Middlewares
{
    public class QueryTimingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<QueryTimingMiddleware> _logger;

        public QueryTimingMiddleware(RequestDelegate next, ILogger<QueryTimingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var totalStopwatch = Stopwatch.StartNew();

            long sqlTime = 0;
            int queryCount = 0;

            var efObserver = new EfQueryObserver(
                duration => Interlocked.Add(ref sqlTime, duration),
                () => Interlocked.Increment(ref queryCount)
            );

            using var subscription = DiagnosticListener.AllListeners.Subscribe(efObserver);

            await _next(context);

            totalStopwatch.Stop();

            _logger.LogInformation("Request {Method} {Path} | Total: {TotalMs}ms | DB: {DbMs}ms | Queries: {QueryCount}",
                context.Request.Method,
                context.Request.Path,
                totalStopwatch.ElapsedMilliseconds,
                TimeSpan.FromTicks(sqlTime).TotalMilliseconds,
                queryCount
            );
        }
    }
}
