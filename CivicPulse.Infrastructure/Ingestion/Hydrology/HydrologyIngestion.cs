using CivicPulse.Core.Interface;

namespace CivicPulse.Infrastructure.Ingestion.Hydrology
{
    public class HydrologyIngestion : IHydrologyIngestion
    {
        public Task<int> IngestAsync(CancellationToken ct = default)
        {
            return Task.FromResult(0);
        }
    }
}
