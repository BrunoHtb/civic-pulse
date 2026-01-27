namespace CivicPulse.Api.Contracts.Read
{
    public record MeasurementPointResponse(
        DateTime TimestampUtc,
        decimal Value
    );
}
