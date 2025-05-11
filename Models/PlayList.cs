namespace MatchGoalAPI.Models
{
	public class PlayList
	{
		[Key]
		public int Id { get; set; }
		[ForeignKey("User")]
		public string UserId { get; set; }
		[ForeignKey("Match")]
		public int MatchId { get; set; }
		public ApplicationUser User { get; set; }
		public Match Match { get; set; }
		public DateTime AddedDate { get; set; }  
	}
}
