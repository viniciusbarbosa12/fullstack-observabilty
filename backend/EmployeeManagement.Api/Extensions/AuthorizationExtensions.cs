namespace EmployeeManagement.Api.Extensions
{
    public static class AuthorizationExtensions
    {
        public static IServiceCollection AddAuthorizationPolicies(this IServiceCollection services)
        {
            services.AddAuthorization(options =>
            {
                options.AddPolicy("IsAdmin", policy =>
                    policy.RequireRole("Admin"));
            });

            return services;
        }
    }
}
