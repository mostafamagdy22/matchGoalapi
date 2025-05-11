
using System.Threading.Tasks;

namespace MatchGoalAPI.Services.Repositories
{
	public class MatchRepository : BaseRepository<Match>,IMatchRepository
	{
		private readonly ApplicationDbContext _context;
		private readonly IConfiguration _configuration;
		private readonly ILogger _logger;
		public MatchRepository(ILogger<MatchRepository> logger,ApplicationDbContext context,IConfiguration configuration):base(context)
		{
			_context = context;
			_configuration = configuration;
			_logger = logger;
		}
		/// <summary>
		/// Add a new match to the database
		/// </summary>
		/// <param name="obj">A <see cref="Match"/></param>
		/// <returns><see cref="bool"/> describe if match added successfully or not</returns>
		public async Task<bool> Add(Match obj)
		{
			try
			{
				await _context.Matches.AddAsync(obj);
				await _context.SaveChangesAsync();
				return true;
			}
			catch (Exception ex)
			{
				return false;
			}
		}
		public override Task<List<Match>> GetAll<TFilter>(int pageSize, int page, TFilter filter, string include)
		{
			return base.GetAll(pageSize, page, filter, include);
		}
		protected override IQueryable<T> ApplyFilter<T, TFilter>(IQueryable<T> query, TFilter filter)
		{
			if (typeof(T) == typeof(Match) && typeof(TFilter) == typeof(MatchFilterDto))
			{
				var matchQuery = query as IQueryable<Match>;
				var matchFilter = filter as MatchFilterDto;

				if (matchQuery == null || matchFilter == null)
					return query;

				if (!string.IsNullOrEmpty(matchFilter.HomeTeamName))
					matchQuery = matchQuery.Where(m => m.HomeTeam.Title.Contains(matchFilter.HomeTeamName));

				if (!string.IsNullOrEmpty(matchFilter.AwayTeamName))
					matchQuery = matchQuery.Where(m => m.AwayTeam.Title.Contains(matchFilter.AwayTeamName));

				if (!string.IsNullOrEmpty(matchFilter.CompetitionName))
					matchQuery = matchQuery.Where(m => m.Competition.Title.Contains(matchFilter.CompetitionName));

				if (!string.IsNullOrEmpty(matchFilter.Stadium))
					matchQuery = matchQuery.Where(m => m.Stadium.Contains(matchFilter.Stadium));

				if (!string.IsNullOrEmpty(matchFilter.Winner))
					matchQuery = matchQuery.Where(m => m.WhoWin.Contains(matchFilter.Winner));

				if (!string.IsNullOrEmpty(matchFilter.HomeTeamShortName))
					matchQuery = matchQuery.Where(m => m.HomeTeam.ShortName.Contains(matchFilter.HomeTeamShortName));

				if (!string.IsNullOrEmpty(matchFilter.AwayTeamShortName))
					matchQuery = matchQuery.Where(m => m.AwayTeam.ShortName.Contains(matchFilter.AwayTeamShortName));

				if (matchFilter.Status != null)
					matchQuery = matchQuery.Where(m => m.Status == matchFilter.Status);

				if (matchFilter.MatchDate != null)
					matchQuery = matchQuery.Where(m => m.MatchDate.Date == matchFilter.MatchDate.Value.Date);

				return matchQuery as IQueryable<T>;
			}

			return query;
		}
		public async Task<Match> GetByID(int id)
		{
			return await _context.Matches.Include(m => m.Competition)
				.Include(m => m.HomeTeam)
				.Include(m => m.AwayTeam)
				.FirstOrDefaultAsync(m => m.ID == id);
		}
		public Task<ICollection<Match>> GetMatchesFromCahche(string source)
		{
			throw new NotImplementedException();
		}
		public async Task<bool> Remove(int id)
		{
			try
			{
				Match match = await _context.Matches.FindAsync(id);
				if (match == null)
					return false;

				_context.Matches.Remove(match);
				await _context.SaveChangesAsync();
				return true;
			}
			catch (Exception ex)
			{
				_logger.LogError($"Error removing match with ID {id}: {ex.Message}");
				return false;
			}
		}

		public async Task<bool> Update(int id,Match obj)
		{
			try
			{
				Match match = await _context.Matches.FindAsync(id);
				if (match == null)
				{
					_logger.LogError($"Match with ID {id} not found.");
					return false;
				}

				if (id != obj.ID)
				{
					_logger.LogError($"Match ID mismatch: {id} != {obj.ID}");
					return false;
				}
				match.HomeTeamID = obj.HomeTeamID;
				match.AwayTeamID = obj.AwayTeamID;
				match.CompetitionID = obj.CompetitionID;
				match.FirstTeamScore = obj.FirstTeamScore;
				match.SecondTeamScore = obj.SecondTeamScore;
				match.MatchDate = obj.MatchDate;
				match.LastScoreUpdate = obj.LastScoreUpdate ?? DateTime.UtcNow;
				match.Stadium = obj.Stadium;
				match.Status = obj.Status;
				match.WinnerID = obj.WinnerID;

				_context.Matches.Update(match);
				await _context.SaveChangesAsync();
				return true;
			}
			catch (Exception ex)
			{
				return false;
			}
		}
		private bool IsValidIncludeNavigtion(string title)
		{
			string[] allowedNavigtionProps = { "HomeTeam", "AwayTeam", "Competition", "Winner" };
			return allowedNavigtionProps.Contains(title,StringComparer.OrdinalIgnoreCase);
		}

		public async Task<int> GetCount(MatchFilterDto filter)
		{
			IQueryable<Match> query = _context.Matches.AsQueryable();

			query = ApplyFilter<Match, MatchFilterDto>(query, filter);
			return await query.CountAsync();
		}
	}
}
