namespace CivicPulse.Core.Interface
{
    public interface IIngestionService
    {
        Task RunAsync(CancellationToken ct = default);
    }
}
