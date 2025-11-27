using AMSample.Domain.Enums;

namespace AMSample.Application.Meteorites.Models;

public class MeteoriteJsonDto
{
    [JsonPropertyName("id")]
    public string ExternalId { get; set; } = string.Empty;
    
    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;
    
    [JsonPropertyName("nametype")]
    public string NameType { get; set; } = string.Empty;
    
    [JsonPropertyName("recclass")]
    public string RecClass { get; set; } = string.Empty;
    
    [JsonPropertyName("mass")]
    public string? Mass { get; set; }
    
    [JsonPropertyName("fall")]
    public string Fall { get; set; } = string.Empty;
    
    [JsonPropertyName("year")]
    public string? Year { get; set; }
    
    [JsonPropertyName("reclat")]
    public string? RecLat { get; set; }
    
    [JsonPropertyName("reclong")]
    public string? RecLong { get; set; }
    
    [JsonPropertyName("geolocation")]
    public GeolocationJsonDto? Geolocation { get; set; }
}

public class MeteoriteJsonDtoProfile : Profile
{
    public MeteoriteJsonDtoProfile()
    {
        CreateMap<MeteoriteJsonDto, Meteorite>()
            .ForMember(d => d.Fall, opt => opt.MapFrom(s => MapFallType(s.Fall)))
            .ForMember(d => d.ExternalId, opt => opt.MapFrom(s => s.ExternalId))
            .ForMember(d => d.Mass, opt => opt.MapFrom(s => TypeHelper.ParseDecimal(s.Mass)))
            .ForMember(d => d.Year, opt => opt.MapFrom(s => TypeHelper.ParseDateTime(s.Year)))
            .ForMember(d => d.RecLat, opt => opt.MapFrom(s => TypeHelper.ParseDecimal(s.RecLat)))
            .ForMember(d => d.RecLong, opt => opt.MapFrom(s => TypeHelper.ParseDecimal(s.RecLong)));
    }
    
    private FallType MapFallType(string? fall)
    {
        return fall?.ToLowerInvariant() switch
        {
            "fell" => FallType.Fell,
            "found" => FallType.Found,
            _ => FallType.Unknown
        };
    }
}