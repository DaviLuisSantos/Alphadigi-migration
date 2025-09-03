using Alphadigi_migration.Application.Commands.Veiculo;
using MediatR;

namespace Alphadigi_migration.Application.Services
{
    public interface IVeiculoAccessMediatorService
    {
        Task<(bool ShouldReturn, string Acesso)> ProcessVeiculoAccessAsync(
            Domain.EntitiesNew.Veiculo veiculo,
            Domain.EntitiesNew.Alphadigi alphadigi,
            DateTime timestamp);
    }

    public class VeiculoAccessMediatorService : IVeiculoAccessMediatorService
    {
        private readonly IMediator _mediator;

        public VeiculoAccessMediatorService(IMediator mediator)
        {
            _mediator = mediator;
        }

        public Task<(bool ShouldReturn, string Acesso)> ProcessVeiculoAccessAsync(
            Domain.EntitiesNew.Veiculo veiculo,
            Domain.EntitiesNew.Alphadigi alphadigi,
            DateTime timestamp)
        {
            return _mediator.Send(new ProcessVeiculoAccessCommand
            {
                Veiculo = veiculo,
                Alphadigi = alphadigi,
                Timestamp = timestamp
            });
        }
    }
}