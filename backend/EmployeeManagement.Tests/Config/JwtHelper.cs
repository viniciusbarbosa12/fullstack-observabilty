using Newtonsoft.Json;
using System.Net.Http.Json;

namespace EmployeeManagement.Tests.Config
{
    public static class JwtHelper
    {
        public static async Task<string> GetAdminTokenAsync(HttpClient client)
        {
            var credentials = new
            {
                Password = "Admin@123",
                UserName = "admin@admin.com",
                Email = "admin@admin.com",
            };

            await client.PostAsJsonAsync("/api/auth/register", credentials);

            var response = await client.PostAsJsonAsync("/api/auth/login", credentials);
            var data = await response.Content.ReadFromJsonAsync<TokenResponse>();
            return data?.Token ?? throw new Exception("Failed to get token");
        }


    }
}
