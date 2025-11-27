namespace AMSample.Infrastructure.Helpers;

public static class MeteoriteQueryHelper
{
    public static IQueryable<Meteorite> ApplyFilters(IQueryable<Meteorite> query, MeteoriteFilters? filters)
    {
        if (filters is null) return query;

        if (filters.YearFrom.HasValue)
        {
            query = query.Where(m => m.Year >= filters.YearFrom.Value);
        }

        if (filters.YearTo.HasValue)
        {
            query = query.Where(m => m.Year <= filters.YearTo.Value);
        }

        if (!string.IsNullOrWhiteSpace(filters.RecClass))
        {
            query = query.Where(m => m.RecClass.Contains(filters.RecClass));
        }

        if (!string.IsNullOrWhiteSpace(filters.NameContains))
        {
            query = query.Where(m => m.Name.Contains(filters.NameContains));
        }

        return query;
    }

    public static IQueryable<Meteorite> ApplySorting(IQueryable<Meteorite> query, MeteoriteFilters? filters)
    {
        if (filters is null) return query.OrderBy(m => m.Name);

        var sortDirection = filters.SortDirection == SortDirection.Ascending;

        return filters.SortBy switch
        {
            SortByType.Year => sortDirection
                ? query.OrderBy(m => m.Year)
                : query.OrderByDescending(m => m.Year),

            SortByType.Mass => sortDirection
                ? query.OrderBy(m => m.Mass)
                : query.OrderByDescending(m => m.Mass),

            SortByType.Name => sortDirection
                ? query.OrderBy(m => m.Name)
                : query.OrderByDescending(m => m.Name),

            _ => query.OrderBy(m => m.Name)
        };
    }

    public static async Task<PaginatedEntity<Meteorite>> GetGroupedResults(
        IQueryable<Meteorite> query,
        int pageNumber,
        int pageSize,
        MeteoriteFilters filters)
    {
        var groupedQuery = filters.GroupBy switch
        {
            GroupByType.Year => query
                .Where(m => m.Year.HasValue)
                .GroupBy(m => m.Year!.Value.Year)
                .Select(g => new EntityGroup
                {
                    Key = g.Key.ToString(),
                    Count = g.Count(),
                    TotalMass = g.Sum(m => m.Mass ?? 0)
                }),

            GroupByType.RecClass => query
                .GroupBy(m => m.RecClass)
                .Select(g => new EntityGroup
                {
                    Key = g.Key,
                    Count = g.Count(),
                    TotalMass = g.Sum(m => m.Mass ?? 0)
                }),

            GroupByType.FallType => query
                .GroupBy(m => m.Fall)
                .Select(g => new EntityGroup
                {
                    Key = g.Key.ToString(),
                    Count = g.Count(),
                    TotalMass = g.Sum(m => m.Mass ?? 0)
                }),

            _ => query
                .GroupBy(m => "All")
                .Select(g => new EntityGroup
                {
                    Key = "All",
                    Count = g.Count(),
                    TotalMass = g.Sum(m => m.Mass ?? 0)
                })
        };

        groupedQuery = ApplyGroupSorting(groupedQuery, filters);

        var totalCount = await groupedQuery.CountAsync();
        var totalPages = (int) Math.Ceiling(totalCount / (double) pageSize);

        var pagedGroups = await groupedQuery
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return new PaginatedEntity<Meteorite>()
        {
            TotalCount = totalCount,
            TotalPages = totalPages,
            GroupedResults = pagedGroups.Select(g => new GroupResult
            {
                GroupKey = g.Key,
                Count = g.Count,
                TotalMass = g.TotalMass
            }).ToList()
        };
    }

    public static IQueryable<EntityGroup> ApplyGroupSorting(
        IQueryable<EntityGroup> groupedQuery,
        MeteoriteFilters? filters)
    {
        if (filters == null) return groupedQuery.OrderBy(g => g.Key);

        var sortDirection = filters.SortDirection == SortDirection.Ascending;

        return filters.SortBy switch
        {
            SortByType.Count => sortDirection
                ? groupedQuery.OrderBy(g => g.Count)
                : groupedQuery.OrderByDescending(g => g.Count),

            SortByType.TotalMass => sortDirection
                ? groupedQuery.OrderBy(g => g.TotalMass)
                : groupedQuery.OrderByDescending(g => g.TotalMass),

            SortByType.Year => sortDirection
                ? groupedQuery.OrderBy(g => g.Key)
                : groupedQuery.OrderByDescending(g => g.Key),

            _ => sortDirection
                ? groupedQuery.OrderBy(g => g.Key)
                : groupedQuery.OrderByDescending(g => g.Key)
        };
    }
}