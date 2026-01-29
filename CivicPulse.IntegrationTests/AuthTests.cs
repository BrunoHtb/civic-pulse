using CivicPulse.IntegrationTests.Infrastructure;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http.Json;
using System.Text;

namespace CivicPulse.IntegrationTests
{
    public class AuthTests : IClassFixture<ApiFactory>
    {
        private readonly HttpClient _client;
        public AuthTests(ApiFactory factory)
        {
            _client = factory.CreateClient();
        }

        [Fact]
        public async Task Login_ReturnsToken()
        {
            var resp = await _client.PostAsJsonAsync("/api/auth/login", new
            {
                Username = "admin",
                Password = "admin123"
            });

            Assert.Equal(HttpStatusCode.OK, resp.StatusCode);

            var json = await resp.Content.ReadFromJsonAsync<Dictionary<string, object>>();
            Assert.NotNull(json);
            Assert.True(json.ContainsKey("accessToken"));
        }
    }
}
