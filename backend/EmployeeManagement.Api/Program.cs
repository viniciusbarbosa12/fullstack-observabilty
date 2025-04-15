using EmployeeManagement.Api.Extensions;
using EmployeeManagement.Api.Middlewares;
using Prometheus;
using Serilog;
using CorrelationId.DependencyInjection;
using Serilog.Formatting.Compact;
using Serilog.Enrichers.Span;
using Serilog.Core;

var builder = WebApplication.CreateBuilder(args);

var serviceProvider = builder.Services.BuildServiceProvider();
var enricher = serviceProvider.GetRequiredService<ILogEventEnricher>();
Log.Logger = new LoggerConfiguration()
    .Enrich.FromLogContext()
    .Enrich.WithProperty("Application", "EmployeeAPI")
    .Enrich.WithProperty("Environment", builder.Environment.EnvironmentName)
    .Enrich.WithCorrelationId()
    .Enrich.With(enricher) 
    .WriteTo.Console(new RenderedCompactJsonFormatter())
    .WriteTo.File("Logs/log.txt", rollingInterval: RollingInterval.Day)
    .WriteTo.Seq("http://seq:80")
    .CreateLogger();


builder.Host.UseSerilog();
builder.Services.AddHttpContextAccessor();
builder.Services.AddCorrelationId();

#region Services
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

builder.Services
    .AddApplicationServices()
    .AddInfrastructureServices(builder.Configuration)
    .AddSwaggerDocs()
    .AddAuthorizationPolicies()
    .AddOpenTelemetryServices()
    .AddAuthorizationCors(); 
#endregion

var app = builder.Build();


#region Middleware Prometheus & Serilog
app.UseMetricServer();     
app.UseHttpMetrics();
app.UseSerilogRequestLogging();
app.UseMiddleware<RequestLoggingMiddleware>();

#endregion

#region Database Migration & Seed with Retry
await app.UseRetryDatabaseConnectionAsync();
#endregion

#region Middleware
app.UseCors("AllowFrontend");
app.UseMiddleware<CorrelationId.CorrelationIdMiddleware>();
app.UseMiddleware<QueryTimingMiddleware>();

app.UseSwagger();              
app.UseSwaggerUI();

if (!app.Environment.IsDevelopment())
{
    app.UseHttpsRedirection();
}

app.UseAuthentication();
app.UseAuthorization();
app.UseGlobalExceptionHandler();
#endregion

app.MapControllers();

app.Run();

public partial class Program { }
