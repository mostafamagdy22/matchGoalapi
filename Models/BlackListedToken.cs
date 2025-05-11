namespace MatchGoalAPI.Models
{
	public class BlackListedToken
	{
		[Key]
		public string Token { get; set; }
		public DateTime ExpireTime { get; set; }
	}
}
