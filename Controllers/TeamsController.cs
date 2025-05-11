using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace MatchGoalAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TeamsController : ControllerBase
    {
		private readonly ITeamRepository _teamRepository;
		private readonly IMapper _mapper;
		public TeamsController(ITeamRepository teamRepository,IMapper mapper)
		{
			_teamRepository = teamRepository;
            _mapper = mapper;
		}
		[HttpGet]
        public async Task<IActionResult> GetAllTeams([FromQuery]int pageSize = 10,[FromQuery] int page = 1
            ,[FromQuery] string filter = "",[FromQuery] string include = "")
        {
            List<Team> teams = await _teamRepository.GetAll(pageSize,page,filter,include);
            if (teams == null || teams.Count == 0) return NotFound();
		    
            List<TeamDto> response = _mapper.Map<List<TeamDto>>(teams);

			return Ok(response);
        }
    }
}
