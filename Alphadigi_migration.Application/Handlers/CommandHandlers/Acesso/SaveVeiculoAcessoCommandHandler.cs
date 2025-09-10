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

    public SaveVeiculoAcessoCommandHandler(IAcessoRepository repository, 
                                           IMediator mediator, 
                                           ILogger<SaveVeiculoAcessoCommandHandler> logger)
    {
        _repository = repository;
        _mediator = mediator;
        _logger = logger;
    }

    public async Task<bool> Handle(SaveVeiculoAcessoCommand request, CancellationToken cancellationToken)
    {
        try
        {
            // VALIDAÇÕES INICIAIS
            if (request.Veiculo == null)
            {
                _logger.LogError("Veículo não pode ser nulo");
                return false;
            }

            if (request.Alphadigi == null)
            {
                _logger.LogError("Alphadigi não pode ser nulo");
                return false;
            }

            if (request.Timestamp == default)
            {
                request.Timestamp = DateTime.Now;
            }

            _logger.LogInformation("Iniciando salvamento de acesso para veículo: {Placa}", request.Veiculo.Placa);
            // 1. Verificar antipassback
            var verifyPassbackCommand = new VerifyAntiPassbackCommand
            {
                Veiculo = request.Veiculo ?? throw new ArgumentNullException(nameof(request.Veiculo)),
                Alphadigi = request.Alphadigi ?? throw new ArgumentNullException(nameof(request.Alphadigi)),
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

                if (int.TryParse(request.Veiculo.Unidade, out int unidadeId))
                {
                    var unidadeQuery = new GetUnidadeByIdQuery { UnidadeId = unidadeId };
                    var unidadeObj = await _mediator.Send(unidadeQuery, cancellationToken);
                    unidade = unidadeObj?.Nome ?? "UNIDADE NAO ENCONTRADA";
                }
                else
                {
                    _logger.LogWarning($"ID da unidade inválido: '{request.Veiculo.Unidade}'");
                    unidade = "ID INVALIDO";
                }



                   
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
    
