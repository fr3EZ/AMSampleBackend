using AMSample.Application.Interfaces;
using AMSample.Application.Meteorites.Models;
using AutoMapper;
using MediatR;

namespace AMSample.Application.Meteorites.Queries;

public record GetMeteoritesQuery(
    int PageNumber,
    int PageSize) : IRequest<MeteoriteDto[]>;

public class GetMeteoritesQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
    : IRequestHandler<GetMeteoritesQuery, MeteoriteDto[]>
{
    public async Task<MeteoriteDto[]> Handle(GetMeteoritesQuery request, CancellationToken cancellationToken)
    {
        var meteorites = await unitOfWork.MeteoritesRepository.GetPaginated(request.PageNumber, request.PageSize);
        
        return mapper.Map<MeteoriteDto[]>(meteorites);
    }
}