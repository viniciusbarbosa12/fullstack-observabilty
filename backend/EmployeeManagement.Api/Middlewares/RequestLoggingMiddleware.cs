using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.Diagnostics;

namespace EmployeeManagement.Api.Middlewares
{
    public class RequestLoggingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<RequestLoggingMiddleware> _logger;

        public RequestLoggingMiddleware(RequestDelegate next, ILogger<RequestLoggingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var correlationId = context.TraceIdentifier;
            var traceId = Activity.Current?.TraceId.ToString();
            var spanId = Activity.Current?.SpanId.ToString();

            _logger.LogInformation("HTTP {Method} {Path} | CorrelationId={CorrelationId} TraceId={TraceId} SpanId={SpanId}",
                context.Request.Method,
                context.Request.Path,
                correlationId,
                traceId,
                spanId);

            await _next(context);

            _logger.LogInformation("HTTP {Method} {Path} responded {StatusCode} | CorrelationId={CorrelationId} TraceId={TraceId} SpanId={SpanId}",
                context.Request.Method,
                context.Request.Path,
                context.Response.StatusCode,
                correlationId,
                traceId,
                spanId);
        }
    }
}
