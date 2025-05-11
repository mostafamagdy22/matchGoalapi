namespace MatchGoalAPI.Dto
{
	public class TeamWithoutMatchesDto
	{
		public int ID { get; set; }
		public string TeamName { get; set; }
		public string TeamShortName { get; set; }
		public int Founded { get; set; }
		public string? Country { get; set; }
	}
}
