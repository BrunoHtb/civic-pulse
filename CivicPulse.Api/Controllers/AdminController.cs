using CivicPulse.Core.Entities;
using CivicPulse.Core.Enums;
using CivicPulse.Core.Interface;
using CivicPulse.Infrastructure.Persistence;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CivicPulse.Api.Controllers
{
    [ApiController]
    [Route("api/admin")]
    public class AdminController : ControllerBase
    {
        private readonly CivicPulseDbContext _db;
        private readonly IWeatherIngestion _weather;

        public AdminController(CivicPulseDbContext db, IWeatherIngestion weather)
        {
            _db = db;
            _weather = weather;
        }

        [Authorize(Policy = "CanIngest")]
        [HttpPost("ingestion/run")]
        public async Task<IActionResult> RunIngestion(CancellationToken ct)
        {
            var source = await _db.Sources.FirstOrDefaultAsync(s => s.Key == "open-meteo", ct);

            if (source is null)
            {
                source = new Source
                {
                    Key = "open-meteo",
                    Name = "Open-Meteo",
                    BaseUrl = "https://api.open-meteo.com",
                    IsActive = true,
                    CreatedAtUtc = DateTime.UtcNow
                };

                _db.Sources.Add(source);
                await _db.SaveChangesAsync(ct);
            }

            var run = new IngestionRun
            {
                SourceId = source.Id,
                StartedAtUtc = DateTime.UtcNow,
                Status = IngestionRunStatus.Running,
                RecordsInserted = 0,
                RecordsUpdated = 0,
                ErrorsCount = 0
            };

            _db.IngestionRuns.Add(run);
            await _db.SaveChangesAsync(ct);

            try
            {
                var result = await _weather.IngestAsync(ct);

                run.RecordsInserted = result.Inserted;
                run.RecordsUpdated = result.Updated;
                run.FinishedAtUtc = DateTime.UtcNow;
                run.Status = IngestionRunStatus.Succeeded;

                await _db.SaveChangesAsync(ct);

                return Ok(new
                {
                    ok = true,
                    inserted = run.RecordsInserted,
                    updated = run.RecordsUpdated,
                    startedAtUtc = run.StartedAtUtc,
                    finishedAtUtc = run.FinishedAtUtc
                });
            }
            catch (Exception)
            {
                run.ErrorsCount = 1;
                run.FinishedAtUtc = DateTime.UtcNow;
                run.Status = IngestionRunStatus.Failed;
                await _db.SaveChangesAsync(ct);
                throw;
            }
        }
    }
}