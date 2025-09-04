using Alphadigi_migration.Application.Commands.MensagemDisplay;
using Alphadigi_migration.Domain.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Alphadigi_migration.Application.Handlers.CommandHandlers.MensagemDisplay;

public class SaveMensagemDisplayCommandHandler : IRequestHandler<SaveMensagemDisplayCommand, bool>
{
    private readonly IMensagemDisplayRepository _repository;
    private readonly ILogger<SaveMensagemDisplayCommandHandler> _logger;

    public SaveMensagemDisplayCommandHandler(
        IMensagemDisplayRepository repository,
        ILogger<SaveMensagemDisplayCommandHandler> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    public async Task<bool> Handle(SaveMensagemDisplayCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Salvando mensagem de display: {Placa} - {Mensagem}", request.Placa, request.Mensagem);

        var mensagemDisplay = new Domain.EntitiesNew.MensagemDisplay(
            request.Placa,
            request.Mensagem,
            request.AlphadigiId,
            request.DataHora,
            request.Prioridade);

        return await _repository.SaveMensagemDisplayAsync(mensagemDisplay);
    }
}