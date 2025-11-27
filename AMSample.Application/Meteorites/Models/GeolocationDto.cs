namespace AMSample.Application.Meteorites.Models;

public class GeolocationDto
{
    public string? Type { get; set; }
    public decimal[]? Coordinates { get; set; }
}

public class GeolocationProfile : Profile
{
    public GeolocationProfile()
    {
        CreateMap<Geolocation, GeolocationDto>()
            .ForMember(d => d.Type, opt => opt.MapFrom(s => s.Type.ToString()));
    }
}