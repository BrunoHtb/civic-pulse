using CivicPulse.Api.Auth;
using CivicPulse.Api.Contracts;
using Microsoft.AspNetCore.Mvc;

namespace CivicPulse.Api.Controller
{
    [ApiController]
    [Route("api/auth")]
    public class AuthController : ControllerBase
    {
        private readonly IConfiguration _cfg;
        private readonly TokenService _tokens;

        public AuthController(IConfiguration cfg, TokenService tokens)
        {
            _cfg = cfg;
            _tokens = tokens;
        }

        [HttpPost("login")]
        public ActionResult<LoginResponse> Login([FromBody] LoginRequest req)
        {
            var user = _cfg["AdminUser:Username"];
            var pass = _cfg["AdminUser:Password"];

            if (req.Username != user || req.Password != pass)
                return Unauthorized(new { message = "Usuário ou senha inválidos" });

            var (token, exp) = _tokens.CreateAdminToken(req.Username);

            return Ok(new LoginResponse(token, exp));
        }
    }
}