using AutoMapper;

namespace MatchGoalAPI.Mapping
{
	public class MatchProfile : Profile
	{
		public MatchProfile()
		{
			CreateMap<Match, MatchDto>()
				.ForMember(dest => dest.HomeTeam, opt => opt.MapFrom(src => src.HomeTeam))
				.ForMember(dest => dest.AwayTeam, opt => opt.MapFrom(src => src.AwayTeam))
				.ForMember(dest => dest.HomeTeamScore, opt => opt.MapFrom(src => src.FirstTeamScore))
				.ForMember(dest => dest.AwayTeamScore, opt => opt.MapFrom(src => src.SecondTeamScore))
				.ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.ToString()))
				.ReverseMap();

			CreateMap<Match, ShallowMatchDto>()
				.ForMember(dest => dest.HomeTeamScore, opt => opt.MapFrom(src => src.FirstTeamScore))
				.ForMember(dest => dest.AwayTeamScore, opt => opt.MapFrom(src => src.SecondTeamScore))
				.ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.ToString()))
				.ReverseMap();

			CreateMap<Match, AddUpdateMatchDto>()
				.ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status))
				.ForMember(dest => dest.Stadium, opt => opt.MapFrom(src => src.Stadium))
				.ForMember(dest => dest.WinnerTeamID, opt => opt.MapFrom(src => src.WinnerID))
				.ForMember(dest => dest.id, opt => opt.MapFrom(src => src.ID))
				.ForMember(dest => dest.HomeTeamID, opt => opt.MapFrom(src => src.HomeTeamID))
				.ForMember(dest => dest.AwayTeamID, opt => opt.MapFrom(src => src.AwayTeamID))
				.ForMember(dest => dest.HomeTeamScore, opt => opt.MapFrom(src => src.FirstTeamScore))
				.ForMember(dest => dest.AwayTeamScore, opt => opt.MapFrom(src => src.SecondTeamScore))
				.ForMember(dest => dest.CompetitionID, opt => opt.MapFrom(src => src.CompetitionID)).ReverseMap();
		}
	}
}
