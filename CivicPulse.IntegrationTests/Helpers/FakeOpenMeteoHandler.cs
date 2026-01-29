using System.Net;
using System.Net.Http;
using System.Text;

namespace CivicPulse.IntegrationTests.Helpers
{
    public sealed class FakeOpenMeteoHandler : HttpMessageHandler
    {
        private readonly string _json;

        public FakeOpenMeteoHandler(string json)
        {
            _json = json;
        }

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var resp = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(_json, Encoding.UTF8, "application/json")
            };

            return Task.FromResult(resp);
        }
    }
}
