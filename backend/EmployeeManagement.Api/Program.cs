using EmployeeManagement.Api.Extensions;

var builder = WebApplication.CreateBuilder(args);

#region Services
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

builder.Services
    .AddApplicationServices()
    .AddInfrastructureServices(builder.Configuration)
    .AddSwaggerDocs()
    .AddAuthorizationPolicies()
    .AddAuthorizationCors(); 
#endregion

var app = builder.Build();

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
