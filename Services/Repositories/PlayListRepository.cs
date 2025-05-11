
namespace MatchGoalAPI.Services.Repositories
{
	public class PlayListRepository : IPlayListRepository
	{
		private readonly ApplicationDbContext _context;

		public PlayListRepository(ApplicationDbContext context)
		{
			_context = context;
		}
		public async Task AddToPlaylistAsync(int matchId, string userId)
		{
			var playlistItem = new PlayList
			{
				MatchId = matchId,
				UserId = userId,
				AddedDate = DateTime.UtcNow
			};
			await _context.PlayLists.AddAsync(playlistItem);
			await _context.SaveChangesAsync();
		}

		public async Task<List<Match>> GetPlayList(string userID)
		{
			List<Match> playLists = await _context.PlayLists.Include(p => p.Match).ThenInclude(m => m.Winner)
				.Include(p => p.Match).ThenInclude(m => m.HomeTeam)
				.Include(p => p.Match).ThenInclude(m => m.AwayTeam)
				.Include(p => p.Match).ThenInclude(m => m.Competition)
				.Where(p => p.UserId == userID).Select(p => p.Match)
				.ToListAsync();

			return playLists;
		}
	}
}
