namespace AMSample.Domain.Common;

public class GroupResult
{
    public string GroupKey { get; set; } = string.Empty;
    public int Count { get; set; }
    public decimal TotalMass { get; set; }
}