using CivicPulse.IntegrationTests.Infrastructure;
using System.Net;

namespace CivicPulse.IntegrationTests
{
    public class StationsTests : IClassFixture<ApiFactory>
    {
        private readonly HttpClient _client;

        public StationsTests(ApiFactory factory)
        {
            _client = factory.CreateClient();
        }

        [Fact]
        public async Task GetStations_ReturnsOK()
        {
            var resp = await _client.GetAsync("/api/stations");
            Assert.Equal(HttpStatusCode.OK, resp.StatusCode);

            var body = await resp.Content.ReadAsStringAsync();
            Assert.False(string.IsNullOrWhiteSpace(body));
        }
    }
}
