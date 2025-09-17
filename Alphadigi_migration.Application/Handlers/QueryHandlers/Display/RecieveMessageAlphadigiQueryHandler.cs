using Alphadigi_migration.Application.Queries.Display;
using Alphadigi_migration.Domain.DTOs.Alphadigi;
using Alphadigi_migration.Domain.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Alphadigi_migration.Application.Handlers.QueryHandlers.Display
{
    public class RecieveMessageAlphadigiQueryHandler : IRequestHandler<RecieveMessageAlphadigiQuery, List<SerialData>>
    {
        private readonly IAlphadigiRepository _alphadigiRepository;
        private readonly ILogger<RecieveMessageAlphadigiQueryHandler> _logger;

        public RecieveMessageAlphadigiQueryHandler(
            IAlphadigiRepository alphadigiRepository,
            ILogger<RecieveMessageAlphadigiQueryHandler> logger)
        {
            _alphadigiRepository = alphadigiRepository;
            _logger = logger;
        }

        public async Task<List<SerialData>> Handle(RecieveMessageAlphadigiQuery request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("INICIANDO RecieveMessageAlphadigiQuery - Linha1: {Linha1}", request.Linha1);
            try
            {
                _logger.LogInformation("Processando mensagem Alphadigi: {Linha1} para dispositivo: {AlphadigiId}",
                    request.Linha1, request.Alphadigi?.Id);

                if (string.IsNullOrEmpty(request.Linha1))
                {
                    _logger.LogWarning("Linha1 está vazia na mensagem Alphadigi");
                    return new List<SerialData>();
                }
                _logger.LogInformation("Criando SerialData...");

                if (request.Alphadigi == null)
                {
                    _logger.LogWarning("Dispositivo Alphadigi não especificado na query");
                    return new List<SerialData>();
                }

                // Processar a mensagem usando o repositório
                var serialDataList = await _alphadigiRepository.ProcessReceivedMessageAsync(
                    request.Linha1,
                    request.Alphadigi.Ip
                    );

                _logger.LogInformation("Mensagem processada com sucesso. {Count} itens de serial data retornados",
                    serialDataList?.Count ?? 0);

                return serialDataList ?? new List<SerialData>();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao processar mensagem Alphadigi: {Linha1}", request.Linha1);
                return new List<SerialData>();
            }
        }
    }
}