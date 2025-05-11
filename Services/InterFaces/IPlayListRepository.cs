namespace MatchGoalAPI.Services.InterFaces
{
	public interface IPlayListRepository
	{
		public Task AddToPlaylistAsync(int matchId, string userId);
		public Task<List<Match>> GetPlayList(string userID);
	}
}
