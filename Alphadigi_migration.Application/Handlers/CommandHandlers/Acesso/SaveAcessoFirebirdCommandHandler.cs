using Alphadigi_migration.Application.Commands.Acesso;
using Alphadigi_migration.Domain.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;


namespace Alphadigi_migration.Application.Handlers.CommandHandlers.Acesso;

public class SaveAcessoFirebirdCommandHandler : IRequestHandler<SaveAcessoFirebirdCommand, bool>
{
    private readonly IAcessoRepository _acessoRepository;
    private readonly ILogger<SaveAcessoFirebirdCommandHandler> _logger;

    public SaveAcessoFirebirdCommandHandler(IAcessoRepository acessoRepository, ILogger<SaveAcessoFirebirdCommandHandler> logger)
    {
        _acessoRepository = acessoRepository;
        _logger = logger;
    }

    public async Task<bool> Handle(SaveAcessoFirebirdCommand request, CancellationToken cancellationToken)
    {
        // Corrigido: Agora, o construtor da entidade Acesso pode ser chamado corretamente
        var acesso = new Domain.EntitiesNew.Acesso(
            local: request.Acesso,
            dataHora: request.Timestamp,
            unidade: request.Unidade,
            placa: request.Placa,
            dadosVeiculo: request.DadosVeiculo, 
            grupoNome: request.GrupoNome,
            ipCamera: request.IpCamera
        );

        return await _acessoRepository.SaveAcessoAsync(acesso);
    }
}