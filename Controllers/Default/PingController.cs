using Microsoft.AspNetCore.Mvc;

namespace SkinsApi.Controllers.Default
{
    [Route("/ping")]
    [ApiController]
    public class PingController : ControllerBase
    {
        [HttpGet]
        public async Task<IActionResult> Ping()
        {
            return Ok("Pong");
        }
    }
}
