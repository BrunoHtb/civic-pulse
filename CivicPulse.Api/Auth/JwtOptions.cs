namespace CivicPulse.Api.Auth
{
    public class JwtOptions
    {
        public string Issuer { get; init; } = default!;
        public string Audience { get; init; } = default!;
        public string Key { get; init; } = default!;
        public int ExpiryMinutes { get; init; } = 60;
    }
}
