using Microsoft.AspNetCore.Mvc;
using SkinsApi.Interfaces.Services;

namespace SkinsApi.Controllers.v1
{
    [Route("/skin/")]
    [ApiController]
    public class AnotherSkinsController ( ISkinService skinService) : ControllerBase
    {
        [HttpGet("{skin_type}/{width}/{user}")]
        [Produces("image/png")]
        [ProducesResponseType(200)]
        public async Task<IActionResult> GetSkin(string skin_type, int width, string user)
        {
            try
            {
                Stream stream;
                switch (skin_type)
                {
                    case "face":
                        stream = (await skinService.GetSkinStreamAsync(user)).GetFace(width);
                        break;
                    case "front":
                        stream = (await skinService.GetSkinStreamAsync(user)).GetBody(width);
                        break;
                    default:
                        stream = (await skinService.GetSkinStreamAsync(user)).GetFull(width);
                        break;

                }
                return File(stream, "image/png");
            }
            catch
            {
                return NotFound();
            }
        }
    }
}
