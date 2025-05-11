using AutoMapper;

namespace MatchGoalAPI.Mapping
{
	public class TeamMapperProfile : Profile
	{
		public TeamMapperProfile() 
		{
			CreateMap<Team, TeamDto>()
				.ForMember(dest => dest.ID, opt => opt.MapFrom(src => src.ID))
				.ForMember(dest => dest.TeamName, opt => opt.MapFrom(src => src.Title))
				.ForMember(dest => dest.Country, opt => opt.MapFrom(src => src.Country))
				.ForMember(dest => dest.Founded, opt => opt.MapFrom(src => src.Founded))
				.ForMember(dest => dest.HomeMatches, opt => opt.MapFrom(src => src.HomeMatches))
				.ForMember(dest => dest.AwayMatches, opt => opt.MapFrom(src => src.AwayMatches))
				.ReverseMap();

			CreateMap<Team, TeamWithoutMatchesDto>()
				.ForMember(dest => dest.TeamName, opt => opt.MapFrom(src => src.Title))
				.ReverseMap();
		}
		
	}
}
