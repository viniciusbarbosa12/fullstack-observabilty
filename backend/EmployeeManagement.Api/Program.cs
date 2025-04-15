using EmployeeManagement.Api.Extensions;
using Prometheus;
using Serilog;

Log.Logger = new LoggerConfiguration()
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .WriteTo.File("Logs/log.txt", rollingInterval: RollingInterval.Day)
    .WriteTo.Seq("http://localhost:5341") // URL do container do Seq (vamos subir depois)
    .CreateLogger();

var builder = WebApplication.CreateBuilder(args);
builder.Host.UseSerilog();

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

#endregion

#region Database Migration & Seed with Retry
await app.UseRetryDatabaseConnectionAsync();
#endregion

#region Middleware
app.UseCors("AllowFrontend"); 

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
