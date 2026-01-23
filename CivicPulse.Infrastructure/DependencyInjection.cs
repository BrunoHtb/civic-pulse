using CivicPulse.Core.Interface;
using CivicPulse.Core.Services;
using CivicPulse.Infrastructure.Ingestion.Hydrology;
using CivicPulse.Infrastructure.Ingestion.Weather;
using CivicPulse.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace CivicPulse.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
        {
            var connectionString = configuration.GetConnectionString("Default") ?? throw new InvalidOperationException("String de conexão 'DEFAULT' não encontrada");
            
            services.AddDbContext<CivicPulseDbContext>(options => options.UseNpgsql(connectionString));
            
            services.AddHttpClient<OpenMeteoClient>(http =>
            {
                http.BaseAddress = new Uri("https://api.open-meteo.com/");
                http.Timeout = TimeSpan.FromSeconds(30);
            });

            services.AddScoped<IWeatherIngestion, WeatherIngestion>();
            services.AddScoped<IHydrologyIngestion, HydrologyIngestion>();

            services.AddScoped<IIngestionService, IngestionService>();

            return services;
        }
    }
}
