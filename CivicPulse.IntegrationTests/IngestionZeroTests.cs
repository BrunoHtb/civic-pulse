using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
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

        var login = await _client.PostAsJsonAsync("/api/auth/login", new
        {
            username = "admin",
            password = "admin123"
        });

        var loginRaw = await login.Content.ReadAsStringAsync();
        Assert.True(login.IsSuccessStatusCode, loginRaw);

        var token = ExtractToken(loginRaw);
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var resp = await _client.PostAsync("/api/admin/ingestion/run", null);
        var raw = await resp.Content.ReadAsStringAsync();
        Assert.True(resp.IsSuccessStatusCode, raw);

        using var doc = JsonDocument.Parse(raw);
        Assert.True(doc.RootElement.TryGetProperty("ok", out var okProp) && okProp.GetBoolean());
    }

    private static string ExtractToken(string json)
    {
        using var doc = JsonDocument.Parse(json);
        var root = doc.RootElement;

        string? token =
            TryGetString(root, "accessToken") ??
            TryGetString(root, "token") ??
            TryGetString(root, "jwt");

        Assert.False(string.IsNullOrWhiteSpace(token));
        return token!;
    }

    private static string? TryGetString(JsonElement obj, string name)
    {
        if (obj.ValueKind != JsonValueKind.Object) return null;
        if (!obj.TryGetProperty(name, out var prop)) return null;
        return prop.ValueKind == JsonValueKind.String ? prop.GetString() : null;
    }
}
