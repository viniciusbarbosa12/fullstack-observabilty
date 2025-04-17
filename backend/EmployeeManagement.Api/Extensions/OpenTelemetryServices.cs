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
                .WithTracing(tracer =>
                {
                    tracer
                        .AddAspNetCoreInstrumentation()
                        .AddHttpClientInstrumentation()
                        .AddSqlClientInstrumentation()
                        .SetResourceBuilder(ResourceBuilder.CreateDefault().AddService("EmployeeManagementApi"))
                        .AddOtlpExporter(o =>
                        {
                            o.Endpoint = new Uri("http://tempo:4317");
                        });
                });

            return services;
        }
    }
}
