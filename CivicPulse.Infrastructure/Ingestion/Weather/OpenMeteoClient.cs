using System.Globalization;
using System.Net.Http.Json;

namespace CivicPulse.Infrastructure.Ingestion.Weather
{
    public class OpenMeteoClient
    {
        private readonly HttpClient _http;

        public OpenMeteoClient(HttpClient http)
        {
            _http = http;
        }

        public async Task<OpenMeteoResponse?> GetHourlyAsync(double lat, double lon, CancellationToken ct = default)
        {
            var latStr = lat.ToString(CultureInfo.InvariantCulture);
            var lonStr = lon.ToString(CultureInfo.InvariantCulture);

            var url = $"v1/forecast?latitude={latStr}&longitude={lonStr}&hourly=temperature_2m,rain&timezone=UTC";

            return await _http.GetFromJsonAsync<OpenMeteoResponse>(url, ct);
        }
    }
}
