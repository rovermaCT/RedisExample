using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RedisCacheExample.Extensions;

namespace RedisCacheExample.Controllers
{
    // Define a complex model for caching testing
    public record UserCacheProfile(int UserId, string Email, string Tier, DateTime CachedAt);

    [Route("api/[controller]")]
    [ApiController]
    public class CacheController : ControllerBase
    {
        // POST: api/cache/save
        [HttpPost("save")]
        public IActionResult SaveUserData([FromBody] UserCacheProfile profile)
        {
            if (profile == null) return BadRequest("Invalid profile payload.");

            // Storing primitive values
            HttpContext.Session.SetString("LastAccessTime", DateTime.UtcNow.ToString("O"));

            // Storing a complex object using our customized extensions
            HttpContext.Session.SetObject("UserProfileData", profile);

            return Ok(new { message = "Data successfully distributed to Redis storage instance." });
        }

        // GET: api/cache/retrieve
        [HttpGet("retrieve")]
        public IActionResult RetrieveUserData()
        {
            // Reading primitive values
            var lastAccess = HttpContext.Session.GetString("LastAccessTime");

            // Reading custom objects via extension methods
            var profile = HttpContext.Session.GetObject<UserCacheProfile>("UserProfileData");

            if (profile == null)
            {
                return NotFound(new { message = "Cache context either expired or does not exist." });
            }

            return Ok(new
            {
                ServerHandledTime = DateTime.UtcNow,
                LastAccessTimestamp = lastAccess,
                CachedObject = profile
            });
        }
    }
}
