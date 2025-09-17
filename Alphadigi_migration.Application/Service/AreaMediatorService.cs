using Alphadigi_migration.Application.Commands.Area;
using Alphadigi_migration.Application.Queries.Area;
using MediatR;

namespace Alphadigi_migration.Application.Service
{
    public interface IAreaMediatorService
    {
        Task<Domain.EntitiesNew.Area> GetByIdAsync(int id);
       // Task<Domain.EntitiesNew.Area> GetByIdAsync(Guid id);
        Task<bool> SyncAreasAsync();
    }

    public class AreaMediatorService : IAreaMediatorService
    {
        private readonly IMediator _mediator;

        public AreaMediatorService(IMediator mediator)
        {
            _mediator = mediator;
        }

        public Task<Domain.EntitiesNew.Area> GetByIdAsync(int id) =>
            _mediator.Send(new GetAreaByIdQuery { id = id });

        //public Task<Domain.EntitiesNew.Area> GetByIdAsync(Guid id) =>
        //    _mediator.Send(new GetAreaByIdGuidQuery { Id = id });

        public Task<bool> SyncAreasAsync() =>
            _mediator.Send(new SyncAreasCommand());
    }
}