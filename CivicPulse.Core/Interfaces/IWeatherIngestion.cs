using CivicPulse.Core.Models;

namespace CivicPulse.Core.Interface
{
    public interface IWeatherIngestion
    {
        Task<IngestionResult> IngestAsync(CancellationToken ct = default);
    }
}
