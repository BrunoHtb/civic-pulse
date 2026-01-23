using CivicPulse.Core.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CivicPulse.Api.Controllers
{
    [ApiController]
    public class AdminController : ControllerBase
    {
        private readonly IIngestionService _ingestion;

        public AdminController(IIngestionService ingestion)
        {
            _ingestion = ingestion;
        }

        [Authorize(Policy = "CanIngest")]
        [HttpPost("ingestion/run")]
        public async Task<IActionResult> RunIngestion(CancellationToken ct)
        {
            await _ingestion.RunAsync(ct);
            return Ok(new { ok = true });
        }
    }
}
