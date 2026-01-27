using CivicPulse.Api.Contracts.Read;
using CivicPulse.Core.Enums;
using CivicPulse.Core.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;


namespace CivicPulse.Api.Controllers
{
    [ApiController]
    [Route("api")]
    public class ReadController : ControllerBase
    {
        private readonly IReadQueries _queries;

        public ReadController(IReadQueries queries)
        {
            _queries = queries;
        }

        [HttpGet("stations")]
        public async Task<ActionResult<List<StationResponse>>> GetStations([FromQuery] StationType? type, [FromQuery] bool onlyActive = true, CancellationToken ct = default)
        {
            var items = await _queries.GetStationsAsync(type, onlyActive, ct);
            return Ok(items.Select(s => new StationResponse(
                s.Id, s.ExternalId, s.Name, s.Type, s.Latitude, s.Longitude, s.Region, s.IsActive
            )).ToList());
        }

        [HttpGet("measurements/latest")]
        public async Task<ActionResult<List<LatestMeasurementResponse>>> GetLatest([FromQuery] Guid stationId, [FromQuery(Name = "var")]VariableType variable, CancellationToken ct = default)
        {
            var latest = await _queries.GetLatestAsync(stationId, variable, ct);
            if (latest is null)
                return NotFound(new { message = "Nenhuma medição encontrada para a estação e variável especificadas." });

            return Ok(new LatestMeasurementResponse(latest.StationId, latest.Variable, latest.TimestampUtc, latest.Value, latest.QualityFlag));
        }

        [HttpGet("measurements/series")]
        public async Task<ActionResult<List<MeasurementPointResponse>>> GetSeries([FromQuery] Guid stationId, [FromQuery(Name = "var")]VariableType variable, [FromQuery] DateTime? from, [FromQuery] DateTime? to, [FromQuery] int limit = 500, CancellationToken ct = default)
        {
            if (limit is 1 or > 500)
                return BadRequest(new { message = "limit deve estar entre 1 e 5000" });

            DateTime? fromUtc = NormalizeUtc(from);
            DateTime? toUtc = NormalizeUtc(to);

            var points = await _queries.GetSeriesAsync(stationId, variable, fromUtc, toUtc, limit, ct);

            return Ok(points.Select(p => new MeasurementPointResponse(p.TimestampUtc, p.Value)).ToList());
        }

        private static DateTime? NormalizeUtc(DateTime? dt)
        {
            if (dt is null) return null;
            if (dt.Value.Kind == DateTimeKind.Utc) return dt.Value;
            if (dt.Value.Kind == DateTimeKind.Local) return dt.Value.ToUniversalTime();
            // Unspecified: assume UTC (ou troque para Local se preferir)
            return DateTime.SpecifyKind(dt.Value, DateTimeKind.Utc);
        }

    }
}
