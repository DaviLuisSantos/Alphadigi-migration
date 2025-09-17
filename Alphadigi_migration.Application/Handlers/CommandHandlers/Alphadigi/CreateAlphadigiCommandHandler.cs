using Alphadigi_migration.Application.Commands.Alphadigi;
using Alphadigi_migration.Domain.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;


namespace Alphadigi_migration.Application.Handlers.CommandHandlers.Alphadigi;

public class CreateAlphadigiCommandHandler : IRequestHandler<CreateAlphadigiCommand, 
                                                             Domain.EntitiesNew.Alphadigi>
{
    private readonly IAlphadigiRepository _repository;
    private readonly ILogger<CreateAlphadigiCommandHandler> _logger;

    public CreateAlphadigiCommandHandler(IAlphadigiRepository repository, 
                                         ILogger<CreateAlphadigiCommandHandler> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    public  async Task<Domain.EntitiesNew.Alphadigi> Handle(CreateAlphadigiCommand request, 
                                                            CancellationToken cancellationToken)
    {
        try
        {
            var dto = new Domain.DTOs.Alphadigi.CreateAlphadigiDTO
            {
                Ip = request.Ip,
                Nome = request.Nome,
                AreaId = request.AreaId,
                Sentido = request.Sentido,
                Estado = request.Estado,
                LinhasDisplay = request.LinhasDisplay,
                FotoEvento = request.FotoEvento
            };

            return await _repository.Create(dto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao criar Alphadigi");
            throw;
        }
    }
}
