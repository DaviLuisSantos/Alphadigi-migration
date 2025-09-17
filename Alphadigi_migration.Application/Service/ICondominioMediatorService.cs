using Alphadigi_migration.Application.Commands.Condominio;
using Alphadigi_migration.Application.Queries.Condominio;
using MediatR;

namespace Alphadigi_migration.Application.Service
{
    public interface ICondominioMediatorService
    {
        Task<Domain.EntitiesNew.Condominio> GetAsync();
        Task<Domain.EntitiesNew.Condominio> GetNewAsync();
        Task<bool> SyncAsync();
    }

    public class CondominioMediatorService : ICondominioMediatorService
    {
        private readonly IMediator _mediator;

        public CondominioMediatorService(IMediator mediator)
        {
            _mediator = mediator;
        }

        public Task<Domain.EntitiesNew.Condominio> GetAsync() =>
            _mediator.Send(new GetCondominioQuery());

        public Task<Domain.EntitiesNew.Condominio> GetNewAsync() =>
            _mediator.Send(new GetNewCondominioQuery());

        public Task<bool> SyncAsync() =>
            _mediator.Send(new SyncCondominioCommand());
    }
}