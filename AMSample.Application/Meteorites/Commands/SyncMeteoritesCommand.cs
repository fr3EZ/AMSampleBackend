using AMSample.Application.Interfaces;
using AMSample.Domain.Entities;
using AutoMapper;
using MediatR;

namespace AMSample.Application.Meteorites.Commands;

public record SyncMeteoritesCommand(IEnumerable<Meteorite> NewMeteoriteData) : IRequest<Unit>;

public class SyncMeteoritesCommandHandler(IUnitOfWork unitOfWork, IMapper mapper)
    : IRequestHandler<SyncMeteoritesCommand, Unit>
{
    public async Task<Unit> Handle(SyncMeteoritesCommand request, CancellationToken cancellationToken)
    {
        var currentData = await unitOfWork.MeteoritesRepository.GetFiltered();
        
        return Unit.Value;
    }
}