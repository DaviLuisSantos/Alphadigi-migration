using Alphadigi_migration.Application.Commands.Visitante;
using Alphadigi_migration.Domain.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Alphadigi_migration.Application.Handlers.CommandHandlers.Visitante
{
    public class ProcessarSaidaVisitanteCommandHandler : IRequestHandler<ProcessarSaidaVisitanteCommand, bool>
    {
        private readonly IVisitanteRepository _visitanteRepository;
        private readonly ILogger<ProcessarSaidaVisitanteCommandHandler> _logger;

        public ProcessarSaidaVisitanteCommandHandler(
            IVisitanteRepository visitanteRepository,
            ILogger<ProcessarSaidaVisitanteCommandHandler> logger)
        {
            _visitanteRepository = visitanteRepository;
            _logger = logger;
        }

        public async Task<bool> Handle(ProcessarSaidaVisitanteCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("🔄 Processando SAÍDA do visitante - Placa: {Placa}, Câmera: {IpCamera}",
                request.Placa, request.IpCamera);

            try
            {
                // Busca o visitante pela placa
                var visitante = await _visitanteRepository.ObterPorPlacaAsync(request.Placa);

                if (visitante == null)
                {
                    _logger.LogWarning("❌ Visitante não encontrado para saída. Placa: {Placa}", request.Placa);
                    return false;
                }

                _logger.LogInformation("🎯 Visitante encontrado para saída: {Nome} - {Placa} - Unidade: {Unidade}",
                    visitante.Nome, visitante.Placa?.Numero, visitante.UnidadeDestino);

                // Exclui o visitante
                var resultado = await _visitanteRepository.ExcluirAsync(visitante.Id);

                if (resultado)
                {
                    _logger.LogInformation("✅ VISITANTE EXCLUÍDO com sucesso: {Nome} - {Placa}",
                        visitante.Nome, visitante.Placa?.Numero);
                }
                else
                {
                    _logger.LogWarning("⚠️  Falha na exclusão do visitante: {Nome}", visitante.Nome);
                }

                return resultado;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Erro ao processar saída do visitante. Placa: {Placa}", request.Placa);
                throw;
            }
        }
    }
}