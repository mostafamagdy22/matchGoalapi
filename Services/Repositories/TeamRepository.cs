


namespace MatchGoalAPI.Services.Repositories
{
	public class TeamRepository : BaseRepository<Team>,ITeamRepository
	{
		public TeamRepository(ApplicationDbContext context) : base(context)
		{
			
		}
		public Task<bool> Add(Team obj)
		{
			throw new NotImplementedException();
		}

		public override Task<List<Team>> GetAll<TFilter>(int pageSize, int page, TFilter filter, string include)
		{
			return base.GetAll(pageSize, page, filter, include);
		}
		protected override IQueryable<T> ApplyFilter<T, TFilter>(IQueryable<T> query, TFilter filter)
		{
			if (typeof(T) == typeof(Team) && typeof(TFilter) == typeof(TeamFilterDto))
			{
				var teamQuery = query as IQueryable<Team>;
				var teamFilter = query as TeamFilterDto;

				if (teamQuery == null || teamFilter == null)
					return query;	

				if (!string.IsNullOrEmpty(teamFilter.Title))
					teamQuery = teamQuery.Where(t => t.Title.Contains(teamFilter.Title));

				if (!string.IsNullOrEmpty(teamFilter.ShortName))
					teamQuery = teamQuery.Where(t => t.ShortName.Contains(teamFilter.ShortName));

				if (!string.IsNullOrEmpty(teamFilter.Country))
					teamQuery = teamQuery.Where(t => t.Country.Contains(teamFilter.Country));

				if (teamFilter.Founded > 0)
					teamQuery = teamQuery.Where(t => t.Founded == teamFilter.Founded);

				return teamQuery as IQueryable<T>;
			}
			return query;
		}

		public Task<Team> GetByID(int id)
		{
			throw new NotImplementedException();
		}

		public Task<bool> Remove(int id)
		{
			throw new NotImplementedException();
		}

		public Task<bool> Update(int id, Team obj)
		{
			throw new NotImplementedException();
		}
	}
}
