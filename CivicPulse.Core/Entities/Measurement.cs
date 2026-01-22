using CivicPulse.Core.Enums;

namespace CivicPulse.Core.Entities
{
    public class Measurement
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid StationId { get; set; }
        public Station Station { get; set; } = default!;
        public Guid SourceId { get; set; }
        public Source Source { get; set; } = default!;
        public VariableType Variable { get; set; }
        public DateTime TimestampUtc { get; set; }
        public decimal Value { get; set; }
        public string? QualityFlag { get; set; }
        public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
    }
}
