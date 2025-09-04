using Alphadigi_migration.Application.Commands.Alphadigi;
using Alphadigi_migration.Domain.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;


namespace Alphadigi_migration.Application.Handlers.CommandHandlers.Alphadigi;

public class UpdateAlphadigiCommandHandler : IRequestHandler<UpdateAlphadigiCommand, Domain.EntitiesNew.Alphadigi>
{
    private readonly IAlphadigiRepository _repository;
    private readonly ILogger<UpdateAlphadigiCommandHandler> _logger;

    public UpdateAlphadigiCommandHandler(IAlphadigiRepository repository, ILogger<UpdateAlphadigiCommandHandler> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    public IAlphadigiRepository Get_repository()
    {
        return _repository;
    }

 

    public async Task<Domain.EntitiesNew.Alphadigi> Handle(UpdateAlphadigiCommand request, 
                                                           CancellationToken cancellationToken)
    {
        try
        {
            var dto = new Domain.DTOs.Alphadigi.UpdateAlphadigiDTO
            {
                Id = request.Id,
                Ip = request.Ip,
                Nome = request.Nome,
                Sentido = request.Sentido,
                Estado = request.Estado,

            };

            var result = await _repository.Update(dto);

            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao atualizar Alphadigi");
            throw;
        }
    }
}
