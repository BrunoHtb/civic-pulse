using CivicPulse.Core.Enums;
using CivicPulse.Core.Interfaces;
using CivicPulse.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace CivicPulse.Infrastructure.Read
{
    public class ReadQueries : IReadQueries
    {
        private readonly CivicPulseDbContext _db;
        public ReadQueries(CivicPulseDbContext db)
        {
            _db = db;
        }

        public async Task<List<StationReadModel>> GetStationsAsync(StationType? type, bool onlyActive, CancellationToken ct)
        {
            var q = _db.Stations.AsNoTracking();

            if (onlyActive)
                q = q.Where(s => s.IsActive);
            if (type is not null)
                q = q.Where(s => s.Type == type);

            return await q.OrderBy(s => s.Name)
                .Select(s => new StationReadModel(s.Id, s.ExternalId, s.Name, s.Type, s.Latitude, s.Longitude, s.Region, s.IsActive))
                .ToListAsync(ct);
        }

        public async Task<LatestMeasurementReadModel?> GetLatestAsync(Guid stationId, VariableType variable, CancellationToken ct)
        {
            return await _db.Measurements.AsNoTracking()
            .Where(m => m.StationId == stationId && m.Variable == variable)
            .OrderByDescending(m => m.TimestampUtc)
            .Select(m => new LatestMeasurementReadModel(m.StationId, m.Variable, m.TimestampUtc, m.Value, m.QualityFlag))
            .FirstOrDefaultAsync(ct);
        }

        public async Task<List<MeasurementPointReadModel>> GetSeriesAsync(Guid stationId, VariableType variable, DateTime? fromUtc, DateTime? toUtc, int limit, CancellationToken ct)
        {
            var q = _db.Measurements.AsNoTracking()
                .Where(m => m.StationId == stationId && m.Variable == variable);

            if (fromUtc is not null) q = q.Where(m => m.TimestampUtc >= fromUtc.Value);
            if (toUtc is not null) q = q.Where(m => m.TimestampUtc <= toUtc.Value);

            return await q.OrderBy(m => m.TimestampUtc)
                .Select(m => new MeasurementPointReadModel(m.TimestampUtc, m.Value))
                .Take(limit)
                .ToListAsync(ct);
        }
    }
}
