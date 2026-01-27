using CivicPulse.Core.Interface;
using CivicPulse.Core.Models;

namespace CivicPulse.Infrastructure.Ingestion.Hydrology
{
    public class HydrologyIngestion : IHydrologyIngestion
    {
        public Task<IngestionResult> IngestAsync(CancellationToken ct = default)
        => Task.FromResult(new IngestionResult(0, 0));
    }
}
