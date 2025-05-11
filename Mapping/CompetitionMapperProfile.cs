using AutoMapper;

namespace MatchGoalAPI.Mapping
{
	public class CompetitionMapperProfile : Profile
	{
		public CompetitionMapperProfile()
		{
			CreateMap<Competition, CompetitionDto>()
				.ForMember(dest => dest.ID, opt => opt.MapFrom(src => src.ID))
				.ForMember(dest => dest.Title, opt => opt.MapFrom(src => src.Title))
				.ForMember(dest => dest.Code, opt => opt.MapFrom(src => src.Code))
				.ForMember(dest => dest.Type, opt => opt.MapFrom(src => src.Type))
				.ReverseMap();
		}
	}
}
