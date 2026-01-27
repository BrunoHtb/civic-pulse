using CivicPulse.Core.Enums;

namespace CivicPulse.Api.Contracts.Read
{
    public record StationResponse(
        Guid Id,
        string ExternalId,
        string Name,
        StationType Type,
        double Latitude,
        double Longitude,
        string? Region,
        bool Isactive
    );
}
