using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.VisualStudio.TestPlatform.TestHost;

namespace CivicPulse.IntegrationTests
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
                    ["Jwt:Audience"] = "CivicPulse.Api",
                    ["Jwt:Key"] = "CHAVE_TESTE_CHAVE_TESTE_CHAVE_TESTE_1234567890"
                };

                cfg.AddInMemoryCollection(overrides);
            });
            return base.CreateHost(builder);
        }
    }
}
