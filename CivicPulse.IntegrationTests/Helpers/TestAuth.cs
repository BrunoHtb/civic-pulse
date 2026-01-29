using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;

namespace CivicPulse.IntegrationTests.Helpers
{
    public static class TestAuth
    {
        public static async Task<string> LoginAndGetTokenAsync(HttpClient client)
        {
            var loginResponse = await client.PostAsJsonAsync("/api/auth/login", new
            {
                username = "admin",
                password = "admin123"
            });

            var raw = await loginResponse.Content.ReadAsStringAsync();
            Assert.True(loginResponse.IsSuccessStatusCode, raw);

            var token = ExtractToken(raw);
            Assert.False(string.IsNullOrWhiteSpace(token));

            return token!;
        }

        public static async Task AuthenticateAsync(HttpClient client)
        {
            var token = await LoginAndGetTokenAsync(client);
            client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", token);
        }

        private static string? ExtractToken(string json)
        {
            using var doc = JsonDocument.Parse(json);
            var root = doc.RootElement;

            return
                TryGetString(root, "accessToken") ??
                TryGetString(root, "token") ??
                TryGetString(root, "jwt");
        }

        private static string? TryGetString(JsonElement obj, string name)
        {
            if (obj.ValueKind != JsonValueKind.Object)
                return null;

            if (!obj.TryGetProperty(name, out var prop))
                return null;

            return prop.ValueKind == JsonValueKind.String
                ? prop.GetString()
                : null;
        }
    }
}
