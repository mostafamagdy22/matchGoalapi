using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace MatchGoalAPI.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class AccountController : ControllerBase
	{
		private readonly UserManager<ApplicationUser> _userManager;
		private readonly IConfiguration _configuration;
		private readonly IAuthRepository _authRepository;
		private readonly DateTime AccessTokenExpiresInMinutes;
		private readonly ILogger<AccountController> _logger;
		public AccountController(ILogger<AccountController> logger,IAuthRepository authRepository,UserManager<ApplicationUser> userManager, IConfiguration configuration)
		{
			_logger = logger;
			_userManager = userManager;
			_configuration = configuration;
			_authRepository = authRepository;
			AccessTokenExpiresInMinutes = DateTime.UtcNow.ToLocalTime().AddMinutes(_configuration.GetValue<int>("Jwt:ExpireTimeByMinutes"));
		}
		[HttpPost("register")]
		public async Task<IActionResult> Register([FromBody] RegisterUserDto registerUserDto)
		{
			if (ModelState.IsValid)
			{
				AuthDto model = await _authRepository.RegisterAsync(registerUserDto);
				if (!model.IsAuthenticated)
					return BadRequest(model.Message);

				SetTokenInCookie(model.Token, AccessTokenExpiresInMinutes);

				return Ok(model);
			}
			return BadRequest(ModelState);
		}
		[HttpGet("refreshToken")]
		public async Task<IActionResult> RefreshToken()
		{
			string refreshToken = Request.Cookies["refreshToken"];
			AuthDto result = await _authRepository.RefreshTokenAsync(refreshToken);

			if (!result.IsAuthenticated)
				return BadRequest(result);

			return Ok(result);
		}
		[HttpPost("RevokeToken")]
		public async Task<IActionResult> RevokeToken([FromBody] RevokeTokenDto revokeTokenDto)
		{
			string revokeToken = revokeTokenDto.Token ?? Request.Cookies["refreshToken"];

			if (revokeToken == null)
				return BadRequest("Token is Invalid");

			bool result = await _authRepository.RevokeTokenAsync(revokeToken);

			if (!result)
				return BadRequest("Token is Invalid");

			return Ok();
		}
		[HttpGet("checkemailexists")]
		public async Task<IActionResult> CheckEmailExists([FromQuery]string email)
		{
			ApplicationUser user = await _userManager.FindByEmailAsync(email);
			if (user == null)
				return Ok(false);

			return Ok(true);
		}
		[HttpPost("login")]
		public async Task<IActionResult> Login([FromBody] LoginUserDto loginUserDto)
		{ 
			if (!ModelState.IsValid) return BadRequest(ModelState);
			var result = await _authRepository.GetTokenAsync(loginUserDto);

			if (!result.IsAuthenticated)
				return BadRequest(result.Message);


			SetTokenInCookie(result.Token, AccessTokenExpiresInMinutes);

			return Ok(result);
		}
		[HttpPost("logout")]
		public async Task<IActionResult> Logout([FromHeader(Name = "Authorization")] string authorization)
		{

			if (string.IsNullOrEmpty(authorization) || !authorization.StartsWith("Bearer "))
				return BadRequest("Invalid token");

			_logger.LogInformation("User logged out at {Time}", DateTime.UtcNow);

			var token = authorization.Substring("Bearer ".Length).Trim();
			var blackListedToke = new BlackListedToken 
			{
				Token = token,
				ExpireTime = DateTime.Now.AddMinutes(_configuration.GetValue<double>("Jwt:ExpireTimeByMinutes"))
			};

			await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
			Response.Headers["Clear-Site-Data"] = "\"cookies\"";
			return Ok(new { message = "Logged out successfully" });
		}
		[HttpGet("isAuthenticated")]
		public IActionResult isAuthenticated()
		{
			return Ok(User.Identity.IsAuthenticated);
		}
		[HttpGet("AddRole")]
		[Authorize(Roles = "admin")]
		public async Task<IActionResult> AddRole(string roleName)
		{
			return Ok(await _authRepository.AddRoleAsync(roleName));
		}
		[HttpPost("AddToRole")]
		[Authorize(AuthenticationSchemes = "Bearer,Cookies", Roles = "Admin")]
		public async Task<IActionResult> AddToRole([FromBody]AddToRoleDto addToRoleDto)
		{
			if (ModelState.IsValid)
			{
				string result = await _authRepository.AddToRoleAsync(addToRoleDto);
				return Ok(result);
			}
			return BadRequest(ModelState);
		}
		private void SetTokenInCookie(string Token,DateTime expires)
		{
			CookieOptions cookieOptions = new CookieOptions()
			{
				Expires = DateTime.Now.AddDays(7),
				HttpOnly = true,
				Secure = true,
				IsEssential = true,
				SameSite = SameSiteMode.None,
			};
			Response.Cookies.Append("accessToken", Token, cookieOptions);
		}
	}
}
