namespace AMSample.Domain.Entities;

public class Geolocation
{
    public string Type { get; private set; }
    public decimal[] Coordinates { get; private set; }
}