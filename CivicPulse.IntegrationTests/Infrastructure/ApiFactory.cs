using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;

namespace CivicPulse.IntegrationTests.Infrastructure
{
    public class ApiFactory : WebApplicationFactory<Program>
    {
        protected override IHost CreateHost(IHostBuilder builder)
        {
            builder.ConfigureAppConfiguration((ctx, cfg) =>
            {
                var overrides = new Dictionary<string, string>
                {
                    ["ConnectionStrings:Default"] = "Host=localhost;Port=5433;Database=civicpulse;Username=postgres;Password=postgres",
                    ["ConnectionString:Redis"] = "localhost:6379",

                    ["Jwt:Issuer"] = "CivicPulse",
                    ["Jwt:Audience"] = "CivicPulse",
                    ["Jwt:Key"] = "SUPER_SECRET_DEV_KEY_CHANGE_ME_32_CHARS_MIN",
                    ["Jwt:ExpiresMinutes"] = "60"
                };

                cfg.AddInMemoryCollection(overrides);
            });
            return base.CreateHost(builder);
        }
    }
}
