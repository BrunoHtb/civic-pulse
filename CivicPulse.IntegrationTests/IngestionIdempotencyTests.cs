using Npgsql;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;

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

        var token = await LoginAsync();
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        // run #1
        await RunIngestionAsync();

        var count1 = await CountAsync(@"SELECT COUNT(*) FROM public.""Measurements"";");

        // run #2
        await RunIngestionAsync();

        var count2 = await CountAsync(@"SELECT COUNT(*) FROM public.""Measurements"";");

        Assert.Equal(count1, count2);
    }

    private async Task<string> LoginAsync()
    {
        var login = await _client.PostAsJsonAsync("/api/auth/login", new
        {
            username = "admin",
            password = "admin123"
        });

        var raw = await login.Content.ReadAsStringAsync();
        Assert.True(login.IsSuccessStatusCode, raw);

        using var doc = JsonDocument.Parse(raw);
        var root = doc.RootElement;

        var token = root.TryGetProperty("accessToken", out var at) ? at.GetString() : null;
        Assert.False(string.IsNullOrWhiteSpace(token));
        return token!;
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
