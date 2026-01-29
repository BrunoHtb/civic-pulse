using CivicPulse.Infrastructure.Ingestion.Weather;
using CivicPulse.IntegrationTests.Helpers;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
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

            builder.ConfigureServices(services =>
            {
                services.RemoveAll<OpenMeteoClient>();

                var fakeJson = """
                {
                  "hourly": {
                    "time": ["2026-01-01T00:00", "2026-01-01T01:00"],
                    "temperature_2m": [25.5, 25.1],
                    "rain": [0.0, 0.2]
                  }
                }
                """;

                var handler = new FakeOpenMeteoHandler(fakeJson);
                var http = new HttpClient(handler)
                {
                    BaseAddress = new Uri("https://api.open-meteo.com/")
                };

                services.AddSingleton(new OpenMeteoClient(http));
            });

            return base.CreateHost(builder);
        }
    }
}
