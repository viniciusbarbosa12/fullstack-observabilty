using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using EmployeeManagement.Infrastructure.Context;

namespace EmployeeManagement.Tests.Config
{
    public class CustomWebApplicationFactory : WebApplicationFactory<Program>
    {
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.ConfigureServices(services =>
            {
                var descriptor = services.SingleOrDefault(
                    d => d.ServiceType == typeof(DbContextOptions<ApplicationDbContext>));

                if (descriptor != null)
                    services.Remove(descriptor);

                services.AddDbContext<ApplicationDbContext>(options =>
                    options.UseSqlServer("Server=localhost;Database=EmployeeManagementDbTests;Trusted_Connection=True;Encrypt=False;"));

                using var scope = services.BuildServiceProvider().CreateScope();
                var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

                
                db.Database.EnsureCreated();
                db.Database.EnsureDeleted();
                db.Database.Migrate();


                DbInitializer.SeedAsync(scope.ServiceProvider).GetAwaiter().GetResult();

            });
        }

        public IServiceProvider Services => Server.Services;

        public void ResetDatabase()
        {
            using var scope = Services.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            db.Database.EnsureDeleted();
            db.Database.Migrate();
        }
    }
}
