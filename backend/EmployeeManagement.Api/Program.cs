using System.Diagnostics;
using Prometheus;
using Serilog;
using Serilog.Sinks.Grafana.Loki;
using CorrelationId.DependencyInjection;

using EmployeeManagement.Api.Diagnostics;
using EmployeeManagement.Api.Extensions;
using EmployeeManagement.Api.Middlewares;
using EmployeeManagement.Api.Metrics;

var builder = WebApplication.CreateBuilder(args);

#region Logging (Serilog + Loki)
Serilog.Debugging.SelfLog.Enable(Console.Error);
builder.Host.UseSerilog((context, services, configuration) =>
{
    configuration
        .Enrich.FromLogContext()
        .Enrich.WithEnvironmentName()
        .Enrich.WithMachineName()
        .Enrich.WithThreadId()
        .Enrich.WithProcessId()
        .WriteTo.Console()
        .WriteTo.GrafanaLoki("http://loki:3100", labels: new[]
        {
            new LokiLabel { Key = "app", Value = "EmployeeManagementApi" },
            new LokiLabel { Key = "env", Value = "dev" }
        });
});

#endregion


#region Infrastructure & Application Services

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

builder.Services
    .AddApplicationServices()
    .AddInfrastructureServices(builder.Configuration)
    .AddSwaggerDocs()
    .AddAuthorizationPolicies()
    .AddOpenTelemetryServices()
    .AddAuthorizationCors();

builder.Services.AddCorrelationId();

#endregion

var app = builder.Build();

#region Observability - EF Core Metrics via Diagnostic Listener

var observer = new EfQueryObserver(
    onQueryExecuted: ticks =>
    {
        var durationMs = TimeSpan.FromTicks(ticks).TotalMilliseconds;
        EfCoreMetrics.DbQueryDuration.Record(durationMs);
    },
    onQueryCounted: () =>
    {
        EfCoreMetrics.IncrementQueryCount();
    });

DiagnosticListener.AllListeners.Subscribe(observer);

#endregion

#region Middleware - Observability First

//  Prometheus - base metrics and HTTP metrics
app.UseMetricServer();                                // Exposes /metrics
app.UseHttpMetrics();                                 // Collects default HTTP metrics

//  Custom dynamic metrics (Controller performance tracking)
app.UseMiddleware<MetricsMiddleware>();

//  Serilog structured logging for each request
app.UseSerilogRequestLogging();

//  Enrich logs with traceId (used by Grafana Loki)
app.UseMiddleware<TraceIdLoggingMiddleware>();

//  Structured log with request/response details
app.UseMiddleware<RequestLoggingMiddleware>();

//  EF Core metrics for query performance
app.UseMiddleware<QueryTimingMiddleware>();

//  Prometheus OTEL endpoint (internal exporter)
app.UseOpenTelemetryPrometheusScrapingEndpoint();

//  CorrelationId support across services
app.UseMiddleware<CorrelationIdMiddleware>();

//  Global error handling - should be one of the last
app.UseGlobalExceptionHandler();

#endregion

#region Middleware - General

app.UseCors("AllowFrontend");

app.UseSwagger();
app.UseSwaggerUI();

if (!app.Environment.IsDevelopment())
{
    app.UseHttpsRedirection();
}

app.UseAuthentication();
app.UseAuthorization();

#endregion

#region Database - Migrations & Retry

await app.UseRetryDatabaseConnectionAsync();

#endregion

#region Endpoint Mapping

app.MapControllers();

#endregion

app.Run();

public partial class Program { }
