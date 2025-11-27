namespace AMSample.Application.Meteorites.Models;

public class GroupResultDto
{
    public string GroupKey { get; set; } = string.Empty;
    public int Count { get; set; }
    public decimal TotalMass { get; set; }
}

public class GroupResultProfile : Profile
{
    public GroupResultProfile()
    {
        CreateMap<GroupResult, GroupResultDto>();
    }
}