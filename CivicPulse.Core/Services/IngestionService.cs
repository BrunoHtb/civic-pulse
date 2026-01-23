
using CivicPulse.Core.Interface;

namespace CivicPulse.Core.Services
{
    public class IngestionService : IIngestionService
    {
        private readonly IWeatherIngestion _weather;
        private readonly IHydrologyIngestion _hydrology;

        public IngestionService(IWeatherIngestion weather, IHydrologyIngestion hydrology)
        {
            _weather = weather;
            _hydrology = hydrology;
        }

        public async Task RunAsync(CancellationToken ct = default)
        {
            await _weather.IngestAsync(ct);
            await _hydrology.IngestAsync(ct);
        }
    }
}
