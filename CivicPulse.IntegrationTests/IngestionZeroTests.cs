using CivicPulse.IntegrationTests.Helpers;
using CivicPulse.IntegrationTests.Infrastructure;
using System.Text.Json;

namespace CivicPulse.IntegrationTests;

public class IngestionZeroTests : IClassFixture<ApiFactory>
{
    private readonly HttpClient _client;

    public IngestionZeroTests(ApiFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task Ingestion_FromZero_CreatesSourceStationMeasurementsAndRun()
    {
        await DbReset.ResetAsync(DbReset.GetTestDbConnectionString());

        await TestAuth.AuthenticateAsync(_client);

        var resp = await _client.PostAsync("/api/admin/ingestion/run", null);
        var raw = await resp.Content.ReadAsStringAsync();
        Assert.True(resp.IsSuccessStatusCode, raw);

        using var doc = JsonDocument.Parse(raw);
        Assert.True(doc.RootElement.TryGetProperty("ok", out var okProp) && okProp.GetBoolean());
    }
}
