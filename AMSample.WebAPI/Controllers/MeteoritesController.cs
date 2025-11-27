namespace AMSample.WebAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class MeteoritesController(IMediator mediator, ILogger<MeteoritesController> logger) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<PaginatedMeteoritesDto>> Get(
        [FromQuery] [BindRequired] int pageNumber,
        [FromQuery] [BindRequired] int pageSize,
        [FromQuery] DateTime? yearFrom = null,
        [FromQuery] DateTime? yearTo = null,
        [FromQuery] string? recClass = null,
        [FromQuery] string? nameContains = null,
        [FromQuery] GroupByType groupBy = GroupByType.None,
        [FromQuery] SortByType sortBy = SortByType.Year,
        [FromQuery] SortDirection sortDirection = SortDirection.Descending)
    {
        var filters = new MeteoriteFilters(yearFrom, yearTo, recClass, nameContains, groupBy, sortBy, sortDirection);

        var paginatedMeteorites = await mediator.Send(new GetMeteoritesQuery(pageNumber, pageSize, filters));

        return Ok(paginatedMeteorites);
    }
}