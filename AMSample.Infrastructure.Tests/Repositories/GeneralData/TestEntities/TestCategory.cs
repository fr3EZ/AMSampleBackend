namespace AMSample.Infrastructure.Tests.Repositories.GeneralData.TestEntities;

public class TestCategory : Entity
{
    [Required] 
    [MaxLength(50)] 
    public string Name { get; set; } = string.Empty;

    public virtual ICollection<TestEntity> TestEntities { get; set; } = new List<TestEntity>();
}