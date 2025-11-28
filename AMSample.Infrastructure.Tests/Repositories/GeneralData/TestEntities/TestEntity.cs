namespace AMSample.Infrastructure.Tests.Repositories.GeneralData.TestEntities;

public class TestEntity : Entity
{
    [Required] 
    [MaxLength(100)] 
    public string Name { get; set; } = string.Empty;

    [MaxLength(500)] 
    public string Description { get; set; } = string.Empty;

    public bool IsActive { get; set; } = true;

    public decimal Price { get; set; }

    public int Quantity { get; set; }

    public DateTime? DateField { get; set; }

    public int? CategoryId { get; set; }

    public virtual TestCategory? Category { get; set; }
}