namespace AMSample.Application.Meteorites.Queries;

public record GetMeteoritesQuery(
    int PageNumber,
    int PageSize,
    MeteoriteFilters Filters) : IRequest<PaginatedMeteoritesDto>;

public class GetMeteoritesQueryHandler(IUnitOfWork unitOfWork, ICacheService cacheService, IMapper mapper)
    : IRequestHandler<GetMeteoritesQuery, PaginatedMeteoritesDto>
{
    public async Task<PaginatedMeteoritesDto> Handle(GetMeteoritesQuery request, CancellationToken cancellationToken)
    {
        var cacheKey = cacheService.GenerateKey(Constants.MeteoritesCachePrefix, new
        {
            request.PageNumber,
            request.PageSize,
            request.Filters.YearFrom,
            request.Filters.YearTo,
            request.Filters.RecClass,
            request.Filters.NameContains,
            request.Filters.GroupBy,
            request.Filters.SortBy,
            request.Filters.SortDirection
        });

        var cachedResult = await cacheService.GetAsync<PaginatedMeteoritesDto>(cacheKey, cancellationToken);

        if (cachedResult != null)
        {
            return cachedResult;
        }

        var meteorites =
            await unitOfWork.Meteorites.GetPaginatedMeteorites(request.PageNumber, request.PageSize, request.Filters);

        var result = mapper.Map<PaginatedMeteoritesDto>(meteorites);

        var cacheOptions = new DistributedCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(10)
        };

        await cacheService.SetAsync(cacheKey, result, cacheOptions, cancellationToken);

        return result;
    }
}