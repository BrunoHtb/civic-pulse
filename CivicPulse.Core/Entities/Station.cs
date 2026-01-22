using CivicPulse.Core.Enums;

namespace CivicPulse.Core.Entities
{
    public class Station
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string ExternalId { get; set; } = default!;
        public string Name { get; set; } = default!;
        public StationType Type { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public string? Region { get; set; }
        public bool IsActive { get; set; } = true;
        public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
    }
}
