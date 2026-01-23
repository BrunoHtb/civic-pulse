namespace CivicPulse.Api.Contracts
{
    public record LoginResponse(string AccessToken, DateTime ExpiresAtUtc);
}
