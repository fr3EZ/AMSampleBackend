namespace AMSample.Application.Meteorites.Dtos;

public class PaginatedMeteoritesDto
{
    public MeteoriteDto[]  Meteorites { get; set; } = Array.Empty<MeteoriteDto>();
    public int TotalCount { get; set; }
    public int TotalPages { get; set; }
    public EntityGroup[] EntityGroups { get; set; } = Array.Empty<EntityGroup>();
}

public class PaginatedMeteoritesProfile : Profile
{
    public PaginatedMeteoritesProfile()
    {
        CreateMap<PaginatedEntity<Meteorite>, PaginatedMeteoritesDto>()
            .ForMember(m => m.Meteorites, opt => opt.MapFrom(src => src.Entities));
    }
}