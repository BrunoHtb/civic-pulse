namespace CivicPulse.Core.Interface
{
    public interface IHydrologyIngestion
    {
        Task<int> IngestAsync(CancellationToken ct = default);
    }
}
