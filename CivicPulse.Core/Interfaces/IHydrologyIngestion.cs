using CivicPulse.Core.Models;

namespace CivicPulse.Core.Interface
{
    public interface IHydrologyIngestion
    {
        Task<IngestionResult> IngestAsync(CancellationToken ct = default);
    }
}
