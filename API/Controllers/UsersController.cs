using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
     // Ensures authorization is required for all actions in this controller
    public class UsersController : ControllerBase
    {
        public UsersController()
        {
            
        } 

        [Authorize]
        [HttpGet("me")]
        public IActionResult GetCurrentUserClaims()
        {
            // Access the claims of the authenticated user
            var claims = User.Claims.ToDictionary(c => c.Type, c => c.Value);

            return Ok(claims); // Return the claims as a response
        }
    }
}
