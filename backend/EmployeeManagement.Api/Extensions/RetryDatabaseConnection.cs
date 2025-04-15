using EmployeeManagement.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;

namespace EmployeeManagement.Api.Extensions
{
    public static class RetryDatabaseConnection
    {
        public static async Task UseRetryDatabaseConnectionAsync(this IApplicationBuilder app)
        {
            using var scope = app.ApplicationServices.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

            var retryCount = 10;
            while (retryCount > 0)
            {
                try
                {
                    db.Database.Migrate();
                    await DbInitializer.SeedAsync(scope.ServiceProvider);
                    break;
                }
                catch (Exception e)
                {
                    retryCount--;
                    Thread.Sleep(2000);
                }
            }
        }
    }
}
