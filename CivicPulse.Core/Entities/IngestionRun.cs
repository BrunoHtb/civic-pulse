using CivicPulse.Core.Enums;

namespace CivicPulse.Core.Entities
{
    public class IngestionRun
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid SourceId { get; set; }
        public Source Source { get; set; } = default!;
        public DateTime StartedAtUtc { get; set; } = DateTime.UtcNow;
        public DateTime? FinishedAtUtc { get; set; }
        public IngestionRunStatus Status { get; set; } = IngestionRunStatus.Running;
        public int RecordsInserted { get; set; }
        public int RecordsUpdated { get; set; }
        public int ErrorsCount { get; set; }
        public string? ErrorSummary { get; set; }
    }

}

