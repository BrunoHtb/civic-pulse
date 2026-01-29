using CivicPulse.IntegrationTests.Helpers;
using CivicPulse.IntegrationTests.Infrastructure;

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
            await TestAuth.AuthenticateAsync(_client);

            var resp = await _client.PostAsync("/api/admin/ingestion/run", null);
            var respRaw = await resp.Content.ReadAsStringAsync();
            Assert.True(resp.IsSuccessStatusCode, respRaw);
        }
    }
}
