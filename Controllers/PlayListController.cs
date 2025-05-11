using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace MatchGoalAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PlayListController : ControllerBase
    {
		private readonly IPlayListRepository _playlistRepository;
		private readonly IMapper _mapper;
		public PlayListController(IMapper mapper,IPlayListRepository playListRepository)
		{
			_playlistRepository = playListRepository;
			_mapper = mapper;
		}
		[HttpPost("addToPlaylist")]
		[Authorize(AuthenticationSchemes ="Bearer,Cookies")]
		public async Task<IActionResult> AddToPlaylist([FromBody] AddToPlaylistRequest request)
		{
			if (request == null || string.IsNullOrEmpty(request.UserId) || request.MatchId <= 0)
			{
				return BadRequest("Invalid data.");
			}

			await _playlistRepository.AddToPlaylistAsync(request.MatchId, request.UserId);

			return Ok(new { message = "Match added to playlist successfully!" });
		}
		[HttpGet("getPlaylist")]
		[Authorize(AuthenticationSchemes = "Bearer,Cookies")]
		public async Task<IActionResult> GetPlayList(string userID)
		{
			List<Match> matches = await _playlistRepository.GetPlayList(userID);
			
			List<MatchDto> matchResponse = _mapper.Map<List<MatchDto>>(matches);

			if (matchResponse == null || matchResponse.Count == 0)
			{
				return NotFound("No matches found in the playlist.");
			}

			return Ok(matchResponse);
		}
	}
}
