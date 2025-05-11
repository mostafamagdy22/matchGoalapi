namespace MatchGoalAPI.Dto
{
	public class MatchDto
	{
		public int ID { get; set; }
		public DateTime MatchDate { get; set; }
		public int? HomeTeamScore { get; set; }
		public int? AwayTeamScore { get; set; }
		public string? Winner { get; set; }
		public string Status { get; set; }
		public string Stadium { get; set; }
		public TeamWithoutMatchesDto? HomeTeam { get; set; }
		public TeamWithoutMatchesDto? AwayTeam { get; set; }
		public CompetitionDto Competition { get; set; }
	}
}
