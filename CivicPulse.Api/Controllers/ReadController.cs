using CivicPulse.Api.Contracts.Read;
using CivicPulse.Core.Enums;
using CivicPulse.Infrastructure.Persistence;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;


namespace CivicPulse.Api.Controllers
{
    [ApiController]
    [Route("api")]
    public class ReadController : ControllerBase
    {
        private readonly CivicPulseDbContext _db;

        public ReadController(CivicPulseDbContext db)
        {
            _db = db;
        }

        [HttpGet("stations")]
        public async Task<ActionResult<List<StationResponse>>> GetStations([FromQuery] StationType? type, [FromQuery] bool onlyActive = true, CancellationToken ct = default)
        {
            var q = _db.Stations.AsNoTracking();

            if (onlyActive)
                q = q.Where(s => s.IsActive);

            if (type is not null)
                q = q.Where(s => s.Type == type);

            var items = await q
                .OrderBy(s => s.Name)
                .Select(s => new StationResponse(
                    s.Id,
                    s.ExternalId,
                    s.Name,
                    s.Type,
                    s.Latitude,
                    s.Longitude,
                    s.Region,
                    s.IsActive))
                .ToListAsync(ct);

            return Ok(items);
        }

        [HttpGet("measurements/latest")]
        public async Task<ActionResult<List<LatestMeasurementResponse>>> GetLatest([FromQuery] Guid stationId, [FromQuery(Name = "var")]VariableType variable, CancellationToken ct = default)
        {
            var latest = await _db.Measurements
                .AsNoTracking()
                .Where(m => m.StationId == stationId && m.Variable == variable)
                .OrderByDescending(m => m.TimestampUtc)
                .Select(m => new LatestMeasurementResponse(
                    m.StationId,
                    m.Variable,
                    m.TimestampUtc,
                    m.Value,
                    m.QualityFlag
                ))
                .FirstOrDefaultAsync(ct);

            if (latest is null)
                return NotFound(new { message = "Nenhuma medição encontrada para essa estação/variável"});

            return Ok(latest);
        }

        [HttpGet("measurements/series")]
        public async Task<ActionResult<List<MeasurementPointResponse>>> GetSeries([FromQuery] Guid stationId, 
            [FromQuery(Name = "var")]VariableType variable, 
            [FromQuery] DateTime? from, 
            [FromQuery] DateTime? to,
            [FromQuery] int limit = 500,
            CancellationToken ct = default)
        {
            if (limit is 1 or > 500)
                return BadRequest(new { message = "limit deve estar entre 1 e 5000" });

            DateTime? fromUtc = NormalizeUtc(from);
            DateTime? toUtc = NormalizeUtc(to);

            var q = _db.Measurements
                .AsNoTracking()
                .Where(m => m.StationId == stationId && m.Variable == variable);

            if (fromUtc is not null)
                q = q.Where(m => m.TimestampUtc >= fromUtc.Value);

            if (toUtc is not null)
                q = q.Where(m => m.TimestampUtc <= toUtc.Value);

            var points = await q
            .OrderBy(m => m.TimestampUtc)
            .Select(m => new MeasurementPointResponse(m.TimestampUtc, m.Value))
            .Take(limit)
            .ToListAsync(ct);

            return Ok(points);
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
