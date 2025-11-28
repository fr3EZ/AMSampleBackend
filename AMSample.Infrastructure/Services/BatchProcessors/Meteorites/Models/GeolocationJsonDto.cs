namespace AMSample.Infrastructure.Services.BatchProcessors.Meteorites.Models;

public class GeolocationJsonDto
{
    [JsonPropertyName("type")]
    public string Type { get; set; } = string.Empty;
    
    [JsonPropertyName("coordinates")]
    public decimal[] Coordinates { get; set; } = Array.Empty<decimal>();
}

public class GeolocationJsonDtoProfile : Profile
{
    public GeolocationJsonDtoProfile()
    {
        CreateMap<GeolocationJsonDto, Geolocation>()
            .ForMember(d => d.Id, opt => opt.Ignore())
            .ForMember(d => d.Type, opt => opt.MapFrom(s => MapGeometryType(s.Type)));
    }
    
    private GeometryType MapGeometryType(string? type)
    {
        return type?.ToLowerInvariant() switch
        {
            "point" => GeometryType.Point,
            "polygon" => GeometryType.Polygon,
            "linestring" => GeometryType.LineString,
            "multipoint" => GeometryType.MultiPoint,
            _ => GeometryType.Unknown
        };
    }
}
