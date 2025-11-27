namespace AMSample.Application.Meteorites.Queries.Validators;

public class GetMeteoritesQueryValidator : AbstractValidator<GetMeteoritesQuery>
{
    public GetMeteoritesQueryValidator()
    {
        RuleFor(q => q.PageNumber)
            .GreaterThan(0).WithMessage("Page number must be greater than 0");

        RuleFor(q => q.PageSize)
            .GreaterThan(0).WithMessage("Page size must be greater than 0");

        RuleFor(q => q.PageSize)
            .LessThanOrEqualTo(100).WithMessage("Page size must be equal 100 or less");

        RuleFor(q => q.Filters.GroupBy)
            .IsInEnum().WithMessage("Group by is invalid");

        RuleFor(q => q.Filters.SortBy)
            .IsInEnum().WithMessage("Sort by is invalid");

        RuleFor(q => q.Filters.SortDirection)
            .IsInEnum().WithMessage("Sort direction is invalid");

        When(q => !string.IsNullOrEmpty(q.Filters.RecClass), () =>
        {
            RuleFor(q => q.Filters.RecClass)
                .Must(HaveCorrectValue).WithMessage("Rec class is invalid");
        });
    }

    private bool HaveCorrectValue(string? value)
    {
        return Constants.AllRecClasses.Contains(value);
    }
}