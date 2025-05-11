using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace MatchGoalAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CompetetionsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;
		public CompetetionsController(IMapper mapper,ApplicationDbContext context)
		{
			_context = context;
            _mapper = mapper;
		}
        [HttpGet]
		public async Task<IActionResult> GetAllAsync()
        {
            List<Competition> competitions = await _context.Competitions.ToListAsync();

            if (competitions == null || competitions.Count == 0)
				return NotFound();

			List<CompetitionDto> competitionsDto = _mapper.Map<List<CompetitionDto>>(competitions);

			return Ok(competitionsDto);
        }
    }
}
