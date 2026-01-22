using Microsoft.EntityFrameworkCore;

namespace CivicPulse.Infrastructure.Persistence
{
    public class CivicPulseDbContext : DbContext
    {
        public CivicPulseDbContext(DbContextOptions<CivicPulseDbContext> options) : base(options)
        {
        }
    }
}
