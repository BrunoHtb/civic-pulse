using CivicPulse.Core.Enums;

namespace CivicPulse.Api.Contracts.Read
{
    public record LatestMeasurementResponse(
        Guid StationId,
        VariableType Variable,
        DateTime TimestampUtc,
        decimal Value,
        string? QualityFlag
    );
}
