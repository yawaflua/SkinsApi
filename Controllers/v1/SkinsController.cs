using Microsoft.AspNetCore.Mvc;
using SkinsApi.Interfaces.Services;

namespace SkinsApi.Controllers.v1
{
    [Route("/api/v1/skin/")]
    [ApiController]

    public class SkinsController(ISkinService skinService) : ControllerBase
    {
        /// <summary>
        /// Get user`s skin
        /// </summary>
        /// <param name="user">nickname or UUID</param>
        /// <returns>Full skin</returns>
        [HttpGet("{user}")]
        [ProducesResponseType(200)]
        [Produces("image/png")]
        public async Task<IActionResult> GetSkinByUser(string user, [FromQuery(Name = "w")] int width = 64)
        {

            return File((await skinService.GetSkinStreamAsync(user)).GetAllSkin(width), "image/png");
        }

        /// <summary>
        /// Get user`s face
        /// </summary>
        /// <param name="user">nickname or UUID</param>
        /// <returns>Face</returns>
        [HttpGet("{user}/face")]
        [ProducesResponseType(200)]
        [Produces("image/png")]
        public async Task<IActionResult> GetSkinFaceByUser(string user, [FromQuery(Name = "w")] int width = 8)
        {

            return File((await skinService.GetSkinStreamAsync(user)).GetFace(width), "image/png");
        }

        /// <summary>
        /// Get user`s front
        /// </summary>
        /// <param name="user">nickname or UUID</param>
        /// <returns>Face</returns>
        [HttpGet("{user}/front")]
        [ProducesResponseType(200)]
        [Produces("image/png")]
        public async Task<IActionResult> GetSkinFrontByUser(string user, [FromQuery(Name = "w")] int width = 128)
        {
            try
            {
                return File((await skinService.GetSkinStreamAsync(user)).GetBody(width), "image/png");
            }
            catch (Exception ex)
            {
                return NotFound();
            }

        }
    }
}
