using Alphadigi_migration.Application.Commands.Acesso;
using Alphadigi_migration.Application.Queries.Acesso;
using Alphadigi_migration.Application.Queries.Veiculo;
using Alphadigi_migration.Domain.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;


namespace Alphadigi_migration.Application.Handlers.CommandHandlers.Acesso;

public class SaveVeiculoAcessoCommandHandler : IRequestHandler<SaveVeiculoAcessoCommand, bool>
{
    private readonly IAcessoRepository _repository;
    private readonly IMediator _mediator;
    private readonly ILogger<SaveVeiculoAcessoCommandHandler> _logger;
    public async Task<bool> Handle(SaveVeiculoAcessoCommand request, CancellationToken cancellationToken)
    {
        try
        {
            // 1. Verificar antipassback
            var verifyPassbackCommand = new VerifyAntiPassbackCommand
            {
                Veiculo = request.Veiculo,
                Alphadigi = request.Alphadigi,
                Timestamp = request.Timestamp
            };

            var estaNoAntiPassback = await _mediator.Send(verifyPassbackCommand, cancellationToken);

            if (estaNoAntiPassback)
            {
                _logger.LogInformation($"Antipassback detectado para veículo {request.Veiculo.Placa}. Acesso não salvo.");
                return false;
            }

            // 2. Salvar imagem se necessário
            string caminhoImg = null;
            if (request.Imagem != null && request.Alphadigi.FotoEvento == true)
            {
                var saveImageCommand = new SaveImageCommand
                {
                    FotoBase64 = request.Imagem,
                    Placa = request.Veiculo.Placa
                };
                caminhoImg = await _mediator.Send(saveImageCommand, cancellationToken);
            }

            // 3. Preparar dados do local
            var localQuery = new PrepareLocalStringQuery { Alphadigi = request.Alphadigi };
            var local = await _mediator.Send(localQuery, cancellationToken);

            // 4. Preparar dados do veículo
            var veiculoDataQuery = new PrepareVeiculoDataStringQuery { Veiculo = request.Veiculo };
            var dadosVeiculo = await _mediator.Send(veiculoDataQuery, cancellationToken);

            // 5. Determinar unidade
            string unidade = "NAO CADASTRADO";
            if (!string.IsNullOrEmpty(request.Veiculo.Unidade))
            {
                var unidadeQuery = new GetUnidadeByIdQuery { UnidadeId = request.Veiculo.Unidade };
                var unidadeObj = await _mediator.Send(unidadeQuery, cancellationToken);
                unidade = unidadeObj?.Nome ?? "UNIDADE NAO ENCONTRADA";
            }

            // 6. Criar e salvar acesso
            var acesso = new Domain.EntitiesNew.Acesso
            (
                local: local,
                dataHora: request.Timestamp,
                unidade: unidade,
                placa: request.Veiculo.Placa,
                dadosVeiculo: dadosVeiculo,
                grupoNome: "",
                foto: caminhoImg,
                ipCamera: request.Alphadigi.Ip

            );

            await _repository.SaveAcessoAsync(acesso);

            _logger.LogInformation($"Acesso salvo para veículo {request.Veiculo.Placa}");
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Erro ao salvar acesso do veículo {request.Veiculo.Placa}");
            throw;
        }
    }
}
    
