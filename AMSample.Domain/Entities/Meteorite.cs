using AMSample.Domain.Enums;

namespace AMSample.Domain.Entities;

public class Meteorite
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