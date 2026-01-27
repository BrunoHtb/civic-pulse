using CivicPulse.Core.Enums;
using CivicPulse.Core.Interfaces;
using Microsoft.Extensions.Caching.Distributed;
using System.Text.Json;

namespace CivicPulse.Infrastructure.Read
{
    public class CachedReadQueries : IReadQueries
    {
        private readonly IReadQueries _inner;
        private readonly IDistributedCache _cache;

        private static readonly TimeSpan StationsTtl = TimeSpan.FromMinutes(10);
        private static readonly TimeSpan LatestTtl = TimeSpan.FromMinutes(1);
        private static readonly TimeSpan SeriesTtl = TimeSpan.FromMinutes(1);

        private static readonly JsonSerializerOptions JsonOptions = new()
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };

        public CachedReadQueries(IReadQueries inner, IDistributedCache cache)
        {
            _inner = inner;
            _cache = cache;
        }

        public Task<List<StationReadModel>> GetStationsAsync(StationType? type, bool onlyActive, CancellationToken ct)
        {
            var key = $"stations:type={type?.ToString() ?? "all"}:active={onlyActive}";
            return GetOrSetAsync(key, () => _inner.GetStationsAsync(type, onlyActive, ct), StationsTtl, ct);
        }

        public Task<LatestMeasurementReadModel?> GetLatestAsync(Guid stationId, VariableType variable, CancellationToken ct)
        {
            var key = $"latest:station={stationId}:var={variable}";
            return GetOrSetAsync(key, () => _inner.GetLatestAsync(stationId, variable, ct), LatestTtl, ct);
        }

        public Task<List<MeasurementPointReadModel>> GetSeriesAsync(Guid stationId, VariableType variable, DateTime? fromUtc, DateTime? toUtc, int limit, CancellationToken ct)
        {
            var fromKey = fromUtc?.ToString("O") ?? "null";
            var toKey = toUtc?.ToString("O") ?? "null";

            var key = $"series:station={stationId}:var={variable}:from={fromKey}:to={toKey}:limit={limit}";
            return GetOrSetAsync(key, () => _inner.GetSeriesAsync(stationId, variable, fromUtc, toUtc, limit, ct), SeriesTtl, ct);
        }

        private async Task<T> GetOrSetAsync<T>(string key, Func<Task<T>> factory, TimeSpan ttl, CancellationToken ct)
        {
            var cached = await _cache.GetStringAsync(key, ct);
            if (cached is not null)
                return JsonSerializer.Deserialize<T>(cached, JsonOptions)!;

            var value = await factory();

            var json = JsonSerializer.Serialize(value, JsonOptions);
            await _cache.SetStringAsync(key, json, new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = ttl
            }, ct);

            return value;
        }
    }
}
