
namespace MatchGoalAPI.Services.Repositories
{
	public abstract class BaseRepository<T> : IBaseRepository<T> where T : class
	{
		protected readonly ApplicationDbContext _context;
		protected readonly DbSet<T> _dbSet;
		protected BaseRepository(ApplicationDbContext context)
		{
			_context = context;
			_dbSet = context.Set<T>();
		}
		public async Task<bool> Add(T obj)
		{
			await _dbSet.AddAsync(obj);
			return true;
		}

		public virtual async Task<List<T>> GetAll<TFilter>(int pageSize,int page,TFilter filter,string include)
		{
			if (pageSize <= 0)
				pageSize = 10;
			if (page <= 0)
				page = 1;

			List<string> includes = new List<string>();
			if (!string.IsNullOrEmpty(include))
			{
				includes = include?.Split(',', StringSplitOptions.RemoveEmptyEntries)
					.Select(i => i.Trim())
					.ToList();
			}
			IQueryable<T> query = _dbSet.AsQueryable();
			foreach (var inc in includes)
			{
				var prop = typeof(T).GetProperties()
	.FirstOrDefault(p => p.Name.Equals(inc, StringComparison.OrdinalIgnoreCase));

				if (prop != null)
					query = query.Include(prop.Name);
			}

			query = ApplyFilter(query, filter);

			query = query
				.Skip((page - 1) * pageSize)
				.Take(pageSize);

			return await query.ToListAsync();
		}

		protected virtual IQueryable<T> ApplyFilter<T,TFilter>(IQueryable<T> query, TFilter filter)
		{	
			return query;
		}
		public Task<T> GetByID(int id)
		{
			throw new NotImplementedException();
		}

		public Task<bool> Remove(int id)
		{
			throw new NotImplementedException();
		}

		public Task<bool> Update(int id, T obj)
		{
			throw new NotImplementedException();
		}
	}
}
