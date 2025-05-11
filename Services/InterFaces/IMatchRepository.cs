namespace MatchGoalAPI.Services.InterFaces
{
	public interface IMatchRepository : IBaseRepository<Match>
	{
		public Task<int> GetCount(MatchFilterDto filter);
	}
}
