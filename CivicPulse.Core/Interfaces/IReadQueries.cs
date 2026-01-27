using CivicPulse.Core.Enums;

namespace CivicPulse.Core.Interfaces
{
    public interface IReadQueries
    {
        Task<List<StationReadModel>> GetStationsAsync(StationType? type, bool onlyActive, CancellationToken ct);
        Task<LatestMeasurementReadModel?> GetLatestAsync(Guid stationId, VariableType variable, CancellationToken ct);
        Task<List<MeasurementPointReadModel>> GetSeriesAsync(Guid stationId, VariableType variable, DateTime? fromUtc, DateTime? toUtc, int limit, CancellationToken ct);
    }

    public record StationReadModel(Guid Id, string ExternalId, string Name, StationType Type, double Latitude, double Longitude, string? Region, bool IsActive);
    public record LatestMeasurementReadModel(Guid StationId, VariableType Variable,  DateTime TimestampUtc, decimal Value, string? QualityFlag);
    public record MeasurementPointReadModel(DateTime TimestampUtc, decimal Value);

}
