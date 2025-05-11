using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Threading.Tasks;

namespace MatchGoalAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
		public UsersController(UserManager<ApplicationUser> userManager)
		{
			_userManager = userManager;
		}
		[HttpGet("user-details")]
        public async Task<IActionResult> GetUserDetails([FromQuery]string userID)
        {
            ApplicationUser user = await _userManager.FindByIdAsync(userID);

			if (user == null)
			{
				return NotFound("User not found");
			}

			IList<string> userRoles = await _userManager.GetRolesAsync(user);
			bool isAdmin = userRoles.Contains("Admin");

			return Ok(new {isAdmin = isAdmin});
        }
    }
}
