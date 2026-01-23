namespace CivicPulse.Core.Interface
{
    public interface IWeatherIngestion
    {
        Task<int> IngestAsync(CancellationToken ct = default);
    }
}
