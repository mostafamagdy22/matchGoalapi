namespace MatchGoalAPI.Services.InterFaces
{
	public interface IBaseRepository<T>
	{
		public Task<List<T>> GetAll<TFilter>(int pageSize, int page, TFilter filter, string include);
		public Task<T> GetByID(int id);
		public Task<bool> Remove(int id);
		public Task<bool> Update(int id,T obj);
		public Task<bool> Add(T obj);
	}
}