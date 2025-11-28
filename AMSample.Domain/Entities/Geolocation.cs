namespace AMSample.Domain.Entities;

public class Geolocation : Entity
{
    public GeometryType Type { get; set; }
    public decimal[]? Coordinates { get; set; }
}