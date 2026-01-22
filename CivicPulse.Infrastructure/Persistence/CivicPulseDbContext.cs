using Microsoft.EntityFrameworkCore;
using CivicPulse.Core.Entities;

namespace CivicPulse.Infrastructure.Persistence
{
    public class CivicPulseDbContext : DbContext
    {
        public CivicPulseDbContext(DbContextOptions<CivicPulseDbContext> options) : base(options)
        {
        }

        public DbSet<Source> Sources => Set<Source>();
        public DbSet<Station> Stations => Set<Station>();
        public DbSet<Measurement> Measurements => Set<Measurement>();
        public DbSet<IngestionRun> IngestionRuns => Set<IngestionRun>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // SOURCE
        modelBuilder.Entity<Source>()
            .HasIndex(x => x.Key)
            .IsUnique();

        modelBuilder.Entity<Source>()
            .Property(x => x.Key)
            .HasMaxLength(80)
            .IsRequired();

        modelBuilder.Entity<Source>()
            .Property(x => x.Name)
            .HasMaxLength(200)
            .IsRequired();

        // STATION
        modelBuilder.Entity<Station>()
            .HasIndex(x => x.ExternalId);

        modelBuilder.Entity<Station>()
            .Property(x => x.ExternalId)
            .HasMaxLength(80)
            .IsRequired();

        modelBuilder.Entity<Station>()
            .Property(x => x.Name)
            .HasMaxLength(200)
            .IsRequired();

        // MEASUREMENT
        modelBuilder.Entity<Measurement>()
            .HasIndex(x => new { x.StationId, x.SourceId, x.Variable, x.TimestampUtc })
            .IsUnique();

        modelBuilder.Entity<Measurement>()
            .Property(x => x.QualityFlag)
            .HasMaxLength(40);

        // RELATIONSHIPS
        modelBuilder.Entity<Measurement>()
            .HasOne(x => x.Station)
            .WithMany()
            .HasForeignKey(x => x.StationId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Measurement>()
            .HasOne(x => x.Source)
            .WithMany()
            .HasForeignKey(x => x.SourceId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<IngestionRun>()
            .HasOne(x => x.Source)
            .WithMany()
            .HasForeignKey(x => x.SourceId)
            .OnDelete(DeleteBehavior.Restrict);
    }
    }
}
