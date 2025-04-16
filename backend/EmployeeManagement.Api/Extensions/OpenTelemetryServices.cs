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
                .WithMetrics(metrics =>
                {
                    metrics
                        .AddRuntimeInstrumentation()
                        .AddAspNetCoreInstrumentation()
                        .AddMeter("EmployeeManagementApi.Metrics")
                        .AddPrometheusExporter();
                })
                .WithTracing(tracing =>
                {
                    tracing
                        .AddAspNetCoreInstrumentation()
                        .AddHttpClientInstrumentation()
                        .AddSqlClientInstrumentation()
                        .AddOtlpExporter(opt =>
                        {
                            opt.Endpoint = new Uri("http://tempo:4317");
                        });
                });

            return services;
        }
    }
}
