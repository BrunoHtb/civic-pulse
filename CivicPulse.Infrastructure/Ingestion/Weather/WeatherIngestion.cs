using CivicPulse.Core.Entities;
using CivicPulse.Core.Enums;
using CivicPulse.Core.Interface;
using CivicPulse.Core.Models;
using CivicPulse.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace CivicPulse.Infrastructure.Ingestion.Weather
{
    public class WeatherIngestion : IWeatherIngestion
    {
        private readonly CivicPulseDbContext _db;
        private readonly OpenMeteoClient _client;

        public WeatherIngestion(CivicPulseDbContext db, OpenMeteoClient client)
        {
            _db = db;
            _client = client;
        }

        public async Task<IngestionResult> IngestAsync(CancellationToken ct = default)
        {
            var source = await _db.Sources.FirstOrDefaultAsync(s => s.Key == "open-meteo", ct);

            if (source is null)
            {
                source = new Source
                {
                    Key = "open-meteo",
                    Name = "Open-Meteo",
                    BaseUrl = "https://open-meteo.com/",
                    IsActive = true,
                };

                _db.Sources.Add(source);
                await _db.SaveChangesAsync(ct);
            }

            var stations = await _db.Stations
                .Where(s => s.Type == StationType.Weather && s.IsActive)
                .ToListAsync(ct);

            if (stations.Count == 0)
            {
                var demo = new Station
                {
                    ExternalId = "demo-curitiba",
                    Name = "Demo - Curitiba",
                    Type = StationType.Weather,
                    Latitude = -25.4284,
                    Longitude = -49.2733,
                    Region = "PR"
                };

                _db.Stations.Add(demo);
                await _db.SaveChangesAsync(ct);
                stations.Add(demo);
            }

            var inserted = 0;
            var updated = 0;

            foreach (var st in stations)
            {
                var resp = await _client.GetHourlyAsync(st.Latitude, st.Longitude, ct);
                if (resp?.Hourly?.Time is null) continue;

                var times = resp.Hourly.Time;
                var temps = resp.Hourly.Temperature_2m;
                var rains = resp.Hourly.Rain;

                for (int i = 0; i < times.Count; i++)
                {
                    if (!DateTimeOffset.TryParse(times[i], out var dto)) continue;
                    var tsUtc = dto.UtcDateTime;

                    // Temperatura
                    if (temps is not null && i < temps.Count)
                    {
                        var r = await UpsertMeasurement(
                            st.Id, source.Id, VariableType.TemperatureC, tsUtc, (decimal)temps[i], ct);

                        inserted += r.Inserted;
                        updated += r.Updated;
                    }

                    // Chuva
                    if (rains is not null && i < rains.Count)
                    {
                        var r = await UpsertMeasurement(
                            st.Id, source.Id, VariableType.RainMm, tsUtc, (decimal)rains[i], ct);

                        inserted += r.Inserted;
                        updated += r.Updated;
                    }
                }
            }

            await _db.SaveChangesAsync(ct);
            return new IngestionResult(inserted, updated);
        }

        private async Task<IngestionResult> UpsertMeasurement(Guid stationId, Guid sourceId, VariableType variable, DateTime tsUtc, decimal value, CancellationToken ct)
        {
            var existing = await _db.Measurements.FirstOrDefaultAsync(m =>
                m.StationId == stationId &&
                m.SourceId == sourceId &&
                m.Variable == variable &&
                m.TimestampUtc == tsUtc, ct);

            if (existing is null)
            {
                _db.Measurements.Add(new Measurement
                {
                    StationId = stationId,
                    SourceId = sourceId,
                    Variable = variable,
                    TimestampUtc = tsUtc,
                    Value = value,
                    QualityFlag = "good"
                });

                return new IngestionResult(Inserted: 1, Updated: 0);
            }

            if (existing.Value != value)
            {
                existing.Value = value;
                return new IngestionResult(Inserted: 0, Updated: 1);
            }

            return new IngestionResult(0, 0);
        }
    }
}
