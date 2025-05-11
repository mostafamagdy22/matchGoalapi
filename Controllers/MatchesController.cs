using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using System.Text;
using System.Text.Json;

namespace MatchGoalAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MatchesController : ControllerBase
    {
		private readonly IMatchRepository _matchRepostory;
		private readonly IMapper _mapper;
		public MatchesController(IMapper mapper,IMatchRepository matchRepostory)
		{
			_matchRepostory = matchRepostory;
			_mapper = mapper;
		}
		/// <summary>
		/// Get all matches from the database
		/// </summary>
		/// <returns>A <see cref="List{Match}"/></returns>
		/// <exception cref="NotFoundObjectResult"></exception>
		[HttpGet]
		public async Task<IActionResult> GetAllMatches(
			[FromQuery] MatchFilterDto filter, 
			[FromQuery] int pageSize = 10,
			[FromQuery] int page = 1,
			[FromQuery] string include = "")
		{
			int totalItems = await _matchRepostory.GetCount(filter);
			List<Match> matches = await _matchRepostory.GetAll(pageSize,page,filter,include);
			if (matches == null || matches.Count == 0)
			{
				return NotFound();
			}
			List<MatchDto> matchesResponse = _mapper.Map<List<MatchDto>>(matches);
			int totalPages = (int)Math.Ceiling((double)totalItems / pageSize);
			var response = new
			{
				data = matchesResponse,
				totalItems,
				page,
				pageSize,
				totalPages,
			};

			return Ok(response);
		}
		[HttpGet("Match/{id}")]
		public async Task<IActionResult> GetMatch(int id)
		{
			if (id <= 0)
				return BadRequest("Invalid ID");

			Match match = await _matchRepostory.GetByID(id);
			if (match == null)
				return NotFound();

			MatchDto matchResponse = _mapper.Map<MatchDto>(match);

			return Ok(matchResponse);
		}
		[HttpPost("AddMatch")]
		[Authorize(Roles = "Admin",AuthenticationSchemes = "Cookies,Bearer")]
		public async Task<IActionResult> AddMatch([FromBody] AddUpdateMatchDto addMatchDto)
		{
			if (ModelState.IsValid)
			{
				Match match = _mapper.Map<Match>(addMatchDto);
				bool result = await _matchRepostory.Add(match);
				if (result)
					return Created("/m",_mapper.Map<MatchDto>(match));

				return StatusCode(StatusCodes.Status500InternalServerError, "Something went wrong");
			}
			return BadRequest(ModelState);
		}
		
		[HttpPut("UpdateMatch/{id}")]
		[Authorize(Roles = "Admin",AuthenticationSchemes = "Cookies,Bearer")]
		public async Task<IActionResult> UpdateMatch([FromRoute]int id, [FromBody]AddUpdateMatchDto updateMatchDto)
		{
			if (ModelState.IsValid)
			{
				Match match = _mapper.Map<Match>(updateMatchDto);
				bool result = await _matchRepostory.Update(id,match);
				if (!result)
					return NotFound();

				MatchDto matchResponse = _mapper.Map<MatchDto>(match);
				return Ok(matchResponse);
			}

			return BadRequest(ModelState);
		}
		[HttpDelete("RemoveMatch/{id}")]
		[Authorize(Roles = "Admin",AuthenticationSchemes = "Bearer,Cookies")]
		public async Task<IActionResult> RemoveMatch([FromRoute]int id)
		{
			bool result = await _matchRepostory.Remove(id);
			if (result)
				return StatusCode(StatusCodes.Status204NoContent);

			return BadRequest();
		}
	}
}