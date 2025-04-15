using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

namespace EmployeeManagement.Api.Extensions
{
    public static class OpenTelemetryServices
    {
        public static IServiceCollection AddOpenTelemetryServices(this IServiceCollection services)
        {
            services.AddOpenTelemetry()
                .ConfigureResource(resource => resource
                    .AddService("EmployeeManagementApi")
                    .AddAttributes(new[]
                    {
                        new KeyValuePair<string, object>("environment", "dev"),
                        new KeyValuePair<string, object>("team", "backend")
                    }))
                .WithTracing(tracing => tracing
                    .AddAspNetCoreInstrumentation()
                    .AddHttpClientInstrumentation()
                    .AddSqlClientInstrumentation()
                    .AddJaegerExporter(o =>
                    {
                        o.AgentHost = "jaeger";
                        o.AgentPort = 6831;
                    }))
                .WithMetrics(metrics => metrics
                    .AddRuntimeInstrumentation()
                    .AddAspNetCoreInstrumentation());

            return services;
        }
    }
}
