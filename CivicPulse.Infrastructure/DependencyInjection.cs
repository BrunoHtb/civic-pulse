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
            
            return services;
        }
    }
}
