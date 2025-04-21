using Serilog.Core;
using Serilog.Events;
using System.Diagnostics;

namespace EmployeeManagement.Api.Extensions
{
    public class HttpRequestEnricher : ILogEventEnricher
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public HttpRequestEnricher(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public void Enrich(LogEvent logEvent, ILogEventPropertyFactory propertyFactory)
        {
            var context = _httpContextAccessor.HttpContext;
            if (context == null) return;

            logEvent.AddPropertyIfAbsent(propertyFactory.CreateProperty("RequestMethod", context.Request.Method));
            logEvent.AddPropertyIfAbsent(propertyFactory.CreateProperty("RequestPath", context.Request.Path));
            logEvent.AddPropertyIfAbsent(propertyFactory.CreateProperty("UserAgent", context.Request.Headers["User-Agent"].ToString()));
            logEvent.AddPropertyIfAbsent(propertyFactory.CreateProperty("CorrelationId", context.TraceIdentifier));

            if (Activity.Current != null)
            {
                logEvent.AddPropertyIfAbsent(propertyFactory.CreateProperty("TraceId", Activity.Current.TraceId.ToString()));
                logEvent.AddPropertyIfAbsent(propertyFactory.CreateProperty("SpanId", Activity.Current.SpanId.ToString()));
            }
        }
    }
}
