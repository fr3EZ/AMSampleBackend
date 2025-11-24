using AMSample.Domain.Entities;
using AMSample.Domain.Enums;
using AutoMapper;

namespace AMSample.Application.Meteorites.Models;

public class MeteoriteDto
{
    public string? Id { get; set; }
    public string? Name { get; set; }
    public NameType NameType { get; set; }
    public string? RecClass { get; set; }
    public decimal? Mass { get; set; }
    public FallType Fall { get; set; }
    public DateTime? Year { get; set; }
    public decimal? RecLat { get; set; }
    public decimal? RecLong { get; set; }
    public Geolocation? Geolocation { get; set; }
}

internal class MeteoriteMap : Profile
{
    public MeteoriteMap()
    {
        CreateMap<Meteorite, MeteoriteDto>();
    }
}