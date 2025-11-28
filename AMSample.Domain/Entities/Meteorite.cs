namespace AMSample.Domain.Entities;

public class Meteorite : Entity
{
    public string ExternalId { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string NameType { get; set; } = string.Empty;
    public string RecClass { get; set; } = string.Empty;
    public decimal? Mass { get; set; }
    public FallType Fall { get; set; }
    public DateTime? Year { get; set; }
    public decimal? RecLat { get; set; }
    public decimal? RecLong { get; set; }
    public Geolocation? Geolocation { get; set; }
}