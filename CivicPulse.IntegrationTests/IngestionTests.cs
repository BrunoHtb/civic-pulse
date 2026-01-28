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
                Username = "admin",
                Password = "admin123"
            });
            Assert.Equal(HttpStatusCode.OK, login.StatusCode);

            var payload = await login.Content.ReadFromJsonAsync<Dictionary<string, object>>();
            var token = payload?["token"]?.ToString();
            Assert.False(string.IsNullOrWhiteSpace(token));

            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var resp = await _client.PostAsync("/api/admin/ingestion/run", null);
            Assert.Equal(HttpStatusCode.OK, resp.StatusCode);

            var body = await resp.Content.ReadAsStringAsync();
            Assert.False(string.IsNullOrWhiteSpace(body));
        }
    }
}
