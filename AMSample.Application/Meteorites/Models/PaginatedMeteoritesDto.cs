namespace AMSample.Application.Meteorites.Models;

public class PaginatedMeteoritesDto
{
    public MeteoriteDto[]  Meteorites { get; set; } = Array.Empty<MeteoriteDto>();
    public int TotalCount { get; set; }
    public int TotalPages { get; set; }
    public GroupResultDto[] GroupedResults { get; set; } = Array.Empty<GroupResultDto>();
}

public class PaginatedMeteoritesProfile : Profile
{
    public PaginatedMeteoritesProfile()
    {
        CreateMap<PaginatedEntity<Meteorite>, PaginatedMeteoritesDto>()
            .ForMember(m => m.Meteorites, opt => opt.MapFrom(src => src.Entities));
    }
}