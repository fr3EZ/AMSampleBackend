namespace AMSample.Application.Meteorites.Dtos;

public class MeteoriteDto
{
    public string? Id { get; set; }
    public string? Name { get; set; }
    public string? NameType { get; set; }
    public string? RecClass { get; set; }
    public decimal? Mass { get; set; }
    public string? Fall { get; set; }
    public DateTime? Year { get; set; }
    public decimal? RecLat { get; set; }
    public decimal? RecLong { get; set; }
    public GeolocationDto? Geolocation { get; set; }
}

public class MeteoriteProfile : Profile
{
    public MeteoriteProfile()
    {
        CreateMap<Meteorite, MeteoriteDto>()
            .ForMember(d => d.Fall, opt => opt.MapFrom(s => s.Fall.ToString()));
    }
}