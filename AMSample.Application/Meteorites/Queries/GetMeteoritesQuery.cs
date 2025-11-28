namespace AMSample.Application.Meteorites.Queries;

public record GetMeteoritesQuery(
    int PageNumber,
    int PageSize,
    MeteoriteFiltersDto FiltersDto) : IRequest<PaginatedMeteoritesDto>;

public class GetMeteoritesQueryHandler(IUnitOfWork unitOfWork, ICacheService cacheService, IMapper mapper)
    : IRequestHandler<GetMeteoritesQuery, PaginatedMeteoritesDto>
{
    public async Task<PaginatedMeteoritesDto> Handle(GetMeteoritesQuery request, CancellationToken cancellationToken)
    {
        var cacheKey = cacheService.GenerateKey(Constants.MeteoritesCachePrefix, new
        {
            request.PageNumber,
            request.PageSize,
            request.FiltersDto.YearFrom,
            request.FiltersDto.YearTo,
            request.FiltersDto.RecClass,
            request.FiltersDto.NameContains,
            request.FiltersDto.GroupBy,
            request.FiltersDto.SortBy,
            request.FiltersDto.SortDirection
        });

        var cachedResult = await cacheService.GetAsync<PaginatedMeteoritesDto>(cacheKey, cancellationToken);

        if (cachedResult != null)
        {
            return cachedResult;
        }

        var meteorites =
            await unitOfWork.Meteorites.GetPaginatedMeteorites(request.PageNumber, request.PageSize, request.FiltersDto);

        var result = mapper.Map<PaginatedMeteoritesDto>(meteorites);

        var cacheOptions = new DistributedCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(10)
        };

        await cacheService.SetAsync(cacheKey, result, cacheOptions, cancellationToken);

        return result;
    }
}