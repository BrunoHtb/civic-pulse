using CivicPulse.IntegrationTests.Helpers;
using CivicPulse.IntegrationTests.Infrastructure;
using Npgsql;

namespace CivicPulse.IntegrationTests;

public class IngestionIdempotencyTests : IClassFixture<ApiFactory>
{
    private readonly HttpClient _client;

    public IngestionIdempotencyTests(ApiFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task Ingestion_Twice_DoesNotDuplicateMeasurements()
    {
        await DbReset.ResetAsync(DbReset.GetTestDbConnectionString());

        await TestAuth.AuthenticateAsync(_client);

        // primeira ingestão #1
        await RunIngestionAsync();
        var count1 = await CountAsync(@"SELECT COUNT(*) FROM public.""Measurements"";");

        // segunda ingestão #2
        await RunIngestionAsync();
        var count2 = await CountAsync(@"SELECT COUNT(*) FROM public.""Measurements"";");

        Assert.Equal(count1, count2);
    }

    private async Task RunIngestionAsync()
    {
        var resp = await _client.PostAsync("/api/admin/ingestion/run", null);
        var raw = await resp.Content.ReadAsStringAsync();
        Assert.True(resp.IsSuccessStatusCode, raw);
    }

    private static async Task<long> CountAsync(string sql)
    {
        await using var conn = new NpgsqlConnection(DbReset.GetTestDbConnectionString());
        await conn.OpenAsync();

        await using var cmd = new NpgsqlCommand(sql, conn);
        var result = await cmd.ExecuteScalarAsync();
        return Convert.ToInt64(result);
    }
}
