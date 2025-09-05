// Alphadigi_migration.Application/Services/AcessoMediatorService.cs
using Alphadigi_migration.Application.Commands.Acesso;
using Alphadigi_migration.Application.Queries.Acesso;
using MediatR;

namespace Alphadigi_migration.Application.Service
{
    public interface IAcessoMediatorService
    {
        Task<bool> SaveVeiculoAcessoAsync(Domain.EntitiesNew.Alphadigi alphadigi, Domain.EntitiesNew.Veiculo veiculo, DateTime timestamp, string imagem);
        Task<bool> VerifyAntiPassbackAsync(Domain.EntitiesNew.Veiculo veiculo, Domain.EntitiesNew.Alphadigi alphadigi, DateTime timestamp);
        Task<string> SaveImageAsync(string fotoBase64, string placa);
        Task<string> PrepareLocalStringAsync(Domain.EntitiesNew.Alphadigi alphadigi);
    }

    public class AcessoMediatorService : IAcessoMediatorService
    {
        private readonly IMediator _mediator;

        public AcessoMediatorService(IMediator mediator)
        {
            _mediator = mediator;
        }

        public Task<bool> SaveVeiculoAcessoAsync(Domain.EntitiesNew.Alphadigi alphadigi, 
                                                 Domain.EntitiesNew.Veiculo veiculo, 
                                                 DateTime timestamp, string imagem) =>
            _mediator.Send(new SaveVeiculoAcessoCommand
            {
                Alphadigi = alphadigi,
                Veiculo = veiculo,
                Timestamp = timestamp,
                Imagem = imagem
            });

        public Task<bool> VerifyAntiPassbackAsync(Domain.EntitiesNew.Veiculo veiculo, 
                                                  Domain.EntitiesNew.Alphadigi alphadigi, 
                                                  DateTime timestamp) =>
            _mediator.Send(new VerifyAntiPassbackCommand
            {
                Veiculo = veiculo,
                Alphadigi = alphadigi,
                Timestamp = timestamp
            });

        public Task<string> SaveImageAsync(string fotoBase64, string placa) =>
            _mediator.Send(new SaveImageCommand
            {
                FotoBase64 = fotoBase64,
                Placa = placa
            });

        public Task<string> PrepareLocalStringAsync(Domain.EntitiesNew.Alphadigi alphadigi) =>
            _mediator.Send(new PrepareLocalStringQuery { Alphadigi = alphadigi });
    }
}