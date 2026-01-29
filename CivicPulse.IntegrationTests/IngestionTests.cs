using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace CivicPulse.IntegrationTests
{
    public class IngestionTests : IClassFixture<ApiFactory>
    {
        private readonly HttpClient _client;

        public IngestionTests(ApiFactory factory)
        {
            _client = factory.CreateClient();
        }

        [Fact]
        public async Task RunIngestion_WithJwt_ReturnsOk()
        {
            var login = await _client.PostAsJsonAsync("/api/auth/login", new
            {
                username = "admin",
                password = "admin123"
            });

            var loginRaw = await login.Content.ReadAsStringAsync();
            Assert.True(login.IsSuccessStatusCode, loginRaw);

            var payload = System.Text.Json.JsonSerializer.Deserialize<Dictionary<string, object>>(loginRaw);
            var token = payload?["accessToken"]?.ToString();
            Assert.False(string.IsNullOrWhiteSpace(token));

            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var resp = await _client.PostAsync("/api/admin/ingestion/run", null);

            var respRaw = await resp.Content.ReadAsStringAsync();
            Assert.True(resp.IsSuccessStatusCode, respRaw);
        }

    }
}
