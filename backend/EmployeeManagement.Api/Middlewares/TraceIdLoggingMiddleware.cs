    using System.Diagnostics;
    using Microsoft.AspNetCore.Http;
    using Serilog.Context;

    namespace EmployeeManagement.Api.Middlewares
    {
        public class TraceIdLoggingMiddleware
        {
            private readonly RequestDelegate _next;

            public TraceIdLoggingMiddleware(RequestDelegate next)
            {
                _next = next;
            }

            public async Task InvokeAsync(HttpContext context)
            {
                var traceId = Activity.Current?.TraceId.ToString();
                var spanId = Activity.Current?.SpanId.ToString();
                var correlationId = context.TraceIdentifier;
                var userId = context.User?.FindFirst("sub")?.Value ?? "anonymous";
                var method = context.Request.Method;
                var path = context.Request.Path;

                var endpoint = context.GetEndpoint();

                var routePattern = (endpoint as RouteEndpoint)?.RoutePattern?.RawText ?? context.Request.Path;


                var routeData = context.GetRouteData();
                var controller = routeData.Values["controller"]?.ToString();
                var action = routeData.Values["action"]?.ToString();

                using (LogContext.PushProperty("traceId", traceId))
                using (LogContext.PushProperty("spanId", spanId))
                using (LogContext.PushProperty("correlationId", correlationId))
                using (LogContext.PushProperty("userId", userId))
                using (LogContext.PushProperty("method", method))
                using (LogContext.PushProperty("path", path))
                using (LogContext.PushProperty("controller", controller))
                using (LogContext.PushProperty("action", action))
                {
                    var activity = Activity.Current;
                    activity?.SetTag("correlationId", correlationId);
                    activity?.SetTag("userId", userId);
                    activity?.SetTag("http.method", method);
                    activity?.SetTag("http.path", path);
                    activity?.SetTag("http.route", routePattern);
                    activity?.SetTag("controller", controller);
                    activity?.SetTag("action", action);

                    await _next(context);
                }
            }

        }
    }
