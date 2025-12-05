using Alphadigi_migration.Application.Commands.Acesso;
using Alphadigi_migration.Application.Commands.Alphadigi;
using Alphadigi_migration.Application.Commands.Display;
using Alphadigi_migration.Application.Commands.PlacaLida;
using Alphadigi_migration.Application.Commands.Veiculo;
using Alphadigi_migration.Application.Commands.Visitante;
using Alphadigi_migration.Application.Queries.Alphadigi;
using Alphadigi_migration.Application.Queries.Veiculo;
using Alphadigi_migration.Application.Service;
using Alphadigi_migration.Domain.DTOs.Alphadigi;
using Alphadigi_migration.Domain.DTOs.Display;
using Alphadigi_migration.Domain.DTOs.Veiculos;
using Alphadigi_migration.Domain.EntitiesNew;
using Alphadigi_migration.Domain.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Alphadigi_migration.Application.Handlers.CommandHandlers.Alphadigi;

public class ProcessPlateCommandHandler : IRequestHandler<ProcessPlateCommand, object>
{
    private readonly IMediator _mediator;
    private readonly ILogger<ProcessPlateCommandHandler> _logger;
    private readonly IVisitanteRepository _visitanteRepository;
    private readonly DisplayService _displayService; 

    public ProcessPlateCommandHandler(
        IMediator mediator,
        ILogger<ProcessPlateCommandHandler> logger,
        IVisitanteRepository visitanteRepository,
        DisplayService displayService)
    {
        _mediator = mediator;
        _logger = logger;
        _visitanteRepository = visitanteRepository;
        _displayService = displayService;
    }

    public async Task<object> Handle(ProcessPlateCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Início do processamento da placa: {Plate}, IP da câmera: {Ip}", request.Plate, request.Ip);

        try
        {
            DateTime timeStamp = DateTime.Now;

            // 1. Buscar câmera
            var camera = await _mediator.Send(new GetOrCreateAlphadigiQuery { Ip = request.Ip });
            if (camera == null)
            {
                _logger.LogError("Câmera não encontrada para o IP {Ip}. Abortando.", request.Ip);
                throw new Exception("Camera não encontrada");
            }



            // 2. Registrar a leitura da placa
            var log = await _mediator.Send(new CreatePlacaLidaCommand
            {
                AlphadigiId = camera.Id,
                Placa = request.Plate,
                DataHora = timeStamp,
                AreaId = camera.AreaId,
                PlacaImg = request.PlateImage,
                CarroImg = request.CarImage
            });
            _logger.LogInformation("Placa lida registrada com ID: {LogId}", log.Id);

            // 3. Buscar veículo (morador)
            var veiculo = await _mediator.Send(new GetVeiculoByPlateQuery { Plate = request.Plate, MinMatchingCharacters = 7 });

            Domain.EntitiesNew.Veiculo veiculoProcessado = null;
            bool isVisitante = false;
            Domain.EntitiesNew.Visitante visitanteAutorizado = null;
            bool isSaidaVisitante = false;

            if (veiculo == null)
            {
                // 3.1 Se não é morador, verifica se é visitante
                _logger.LogInformation("🔍 Veículo não encontrado como morador. Verificando se é visitante: {Plate}", request.Plate);

                visitanteAutorizado = await _visitanteRepository.ObterPorPlacaAsync(request.Plate);

                if (visitanteAutorizado != null)
                {
                    _logger.LogInformation("🎯 VISITANTE ENCONTRADO: {Nome} - {Unidade}",
                        visitanteAutorizado.Nome, visitanteAutorizado.UnidadeDestino);

                    // VERIFICA SE É UMA SAÍDA
                    bool isSaida = !camera.Sentido; // Sentido = false → Saída
                    _logger.LogInformation("📷 Configuração da câmera - Sentido: {Sentido} (True=Entrada, False=Saída)", camera.Sentido);

                    isVisitante = true;
                    isSaidaVisitante = isSaida;

                    // CORREÇÃO: SEMPRE cria veículo temporário, mesmo para saída
                    var marcaVisitante = !string.IsNullOrWhiteSpace(visitanteAutorizado.Marca) ? visitanteAutorizado.Marca : "INDEFINIDO";
                    var modeloVisitante = !string.IsNullOrWhiteSpace(visitanteAutorizado.Modelo) ? visitanteAutorizado.Modelo : "INDEFINIDO";
                    var corVisitante = !string.IsNullOrWhiteSpace(visitanteAutorizado.Cor) ? visitanteAutorizado.Cor : "INDEFINIDO";
                    var unidadeVisitante = !string.IsNullOrWhiteSpace(visitanteAutorizado.UnidadeDestino) ? visitanteAutorizado.UnidadeDestino : "VISITANTE";

                    _logger.LogInformation("🚙 Dados do veículo - Marca: {Marca}, Modelo: {Modelo}, Cor: {Cor}",
                        marcaVisitante, modeloVisitante, corVisitante);

                    // Cria veículo temporário para o visitante (tanto entrada quanto saída)
                    veiculoProcessado = new Domain.EntitiesNew.Veiculo(
                        request.Plate,
                        unidadeVisitante,
                        marcaVisitante,
                        modeloVisitante,
                        corVisitante
                    );
                    isVisitante = true;
                    isSaidaVisitante = isSaida;

                    if (isSaida)
                    {
                        _logger.LogInformation("🚗 DETECTADA SAÍDA do visitante: {Nome} - {Placa}",
                            visitanteAutorizado.Nome, visitanteAutorizado.Placa?.Numero);
                    }
                    else
                    {
                        _logger.LogInformation("🚪 DETECTADA ENTRADA do visitante: {Nome} - {Placa}",
                            visitanteAutorizado.Nome, visitanteAutorizado.Placa?.Numero);

                        // Registra entrada do visitante se ainda não registrada
                        if (!visitanteAutorizado.DataHoraEntrada.HasValue)
                        {
                            _logger.LogInformation("📝 Registrando entrada do visitante: {Nome}", visitanteAutorizado.Nome);
                            visitanteAutorizado.RegistrarEntrada("Sistema LPR Automático");
                            await _visitanteRepository.AtualizarAsync(visitanteAutorizado);
                            _logger.LogInformation("✅ Entrada registrada para visitante: {Nome}", visitanteAutorizado.Nome);
                        }
                        else
                        {
                            _logger.LogInformation("ℹ️  Visitante já possui entrada registrada: {Data}",
                                visitanteAutorizado.DataHoraEntrada.Value.ToString("dd/MM/yyyy HH:mm"));
                        }
                    }
                }
                else
                {
                    // Não é morador nem visitante
                    _logger.LogInformation("❌ Placa {Plate} não é morador nem visitante autorizado", request.Plate);
                    veiculoProcessado = new Domain.EntitiesNew.Veiculo(
                        request.Plate,
                        "SEM UNIDADE",
                        "NAO CADASTRADO",
                        "NAO CADASTRADO",
                        "NAO CADASTRADO"
                    );
                }
            }
            else
            {
                // É morador
                veiculoProcessado = veiculo;
                _logger.LogInformation("🏠 Veículo encontrado como morador: {Plate} - {Unidade}",
                    veiculo.Placa?.Numero, veiculo.Unidade);
            }

            // 4. Se veículo processado é nulo, retorna erro
            if (veiculoProcessado == null)
            {
                _logger.LogWarning("⚠️  Veículo processado é nulo. Retornando acesso negado.");
                return new
                {
                    Placa = request.Plate,
                    Liberado = false,
                    Acesso = "ERRO: Veículo não processado",
                    Tipo = "ERRO"
                };
            }

            _logger.LogInformation("🔐 Processando regras de acesso para: {Plate} (Tipo: {Tipo})",
                request.Plate, isVisitante ? "VISITANTE" : "MORADOR");

            var accessResult = await _mediator.Send(new ProcessVeiculoAccessCommand
            {
                Veiculo = veiculoProcessado,
                Alphadigi = camera,
                Timestamp = timeStamp,
                IsVisitante = isVisitante
            });

            _logger.LogInformation("📋 Resultado do acesso: Liberado={Liberado}, Motivo={Acesso}",
                accessResult.ShouldReturn, accessResult.Acesso);

            // 5. Ajusta a mensagem de acesso para visitantes
            string mensagemAcesso = accessResult.Acesso;
            if (isVisitante && visitanteAutorizado != null)
            {
                if (accessResult.ShouldReturn)
                {
                    if (isSaidaVisitante)
                    {
                        mensagemAcesso = $"SAÍDA - VISITANTE: {visitanteAutorizado.Nome}";
                        _logger.LogInformation("🎉 SAÍDA LIBERADA para VISITANTE {Plate}: {Nome}",
                            request.Plate, visitanteAutorizado.Nome);
                    }
                    else
                    {
                        mensagemAcesso = $"VISITANTE: {visitanteAutorizado.Nome} - {visitanteAutorizado.UnidadeDestino}";
                        _logger.LogInformation("🎉 ACESSO LIBERADO para VISITANTE {Plate}: {Nome}",
                            request.Plate, visitanteAutorizado.Nome);
                    }
                }
                else
                {
                    _logger.LogWarning("🚫 ACESSO NEGADO para VISITANTE {Plate}: {Nome}. Motivo: {Motivo}",
                        request.Plate, visitanteAutorizado.Nome, accessResult.Acesso);
                }
            }

            // 6. Salvar acesso se liberado
            if (accessResult.ShouldReturn)
            {
                _logger.LogInformation("💾 Salvando acesso LIBERADO para veículo {Plate}", request.Plate);
                await _mediator.Send(new SaveVeiculoAcessoCommand
                {
                    Veiculo = veiculoProcessado,
                    Alphadigi = camera,
                    Timestamp = timeStamp,
                    Imagem = request.CarImage,
                    IsVisitante = isVisitante
                });
            }
            else
            {
                _logger.LogInformation("🚫 Acesso NEGADO para veículo {Plate}. Motivo: {Acesso}", request.Plate, accessResult.Acesso);
            }

            // 7. Atualizar log da placa lida
            await _mediator.Send(new UpdatePlacaLidaAcessoCommand
            {
                PlacaLidaId = log.Id,
                Liberado = accessResult.ShouldReturn,
                Acesso = mensagemAcesso,
                IsVisitante = isVisitante
            });

            // 8. Enviar para display (com informações do visitante se aplicável)

            var serialData = await _displayService.RecieveMessageAlphadigi(
            request.Plate,
            mensagemAcesso,  // Use a mensagem corrigida
            camera);

            _logger.LogInformation("📦 Dados do display gerados: {Count} pacotes", serialData?.Count ?? 0);


            // 9. Enviar dados para monitor
            var dadosVeiculoDTO = new DadosVeiculoMonitorDTO
            {
                Placa = veiculoProcessado.Placa?.Numero ?? request.Plate,
                Unidade = veiculoProcessado.Unidade ?? "NAO CADASTRADO",
                Ip = request.Ip,
                Acesso = mensagemAcesso,
                HoraAcesso = timeStamp,
                Modelo = veiculoProcessado.Modelo ?? "INDEFINIDO",
                Marca = veiculoProcessado.Marca ?? "INDEFINIDO",
                Cor = veiculoProcessado.Cor ?? "INDEFINIDO",
                IsVisitante = isVisitante,
                VisitanteNome = visitanteAutorizado?.Nome,
                VisitanteUnidade = visitanteAutorizado?.UnidadeDestino
            };

            string dadosVeiculoStr = isVisitante && visitanteAutorizado != null
                ? $"VISITANTE: {visitanteAutorizado.Nome} - {visitanteAutorizado.UnidadeDestino}"
                : $"{dadosVeiculoDTO.Modelo} - {dadosVeiculoDTO.Marca} - {dadosVeiculoDTO.Cor}";

            await _mediator.Send(new SendMonitorAcessoLinearCommand
            {
                DadosVeiculo = dadosVeiculoDTO,
                IpCamera = request.Ip,
                Acesso = mensagemAcesso,
                Timestamp = timeStamp,
                DadosVeiculoStr = dadosVeiculoStr
            });

            // 10. 🆕 EXCLUSÃO DO VISITANTE APÓS LIBERAR A CANCELA (apenas para saída)

            await _mediator.Send(new UpdatePlacaLidaAcessoCommand
            {
                PlacaLidaId = log.Id,
                Liberado = accessResult.ShouldReturn,
                Acesso = mensagemAcesso,
                IsVisitante = isVisitante
            });
            if (isVisitante && visitanteAutorizado != null && isSaidaVisitante && accessResult.ShouldReturn)
            {
                _logger.LogInformation("🗑️  Processando EXCLUSÃO do visitante após saída liberada: {Nome}", visitanteAutorizado.Nome);

                try
                {
                    var resultadoExclusao = await _mediator.Send(new ProcessarSaidaVisitanteCommand(
                        request.Plate,
                        camera.Id,
                        request.Ip
                    ));

                    if (resultadoExclusao)
                    {
                        _logger.LogInformation("✅ VISITANTE EXCLUÍDO com sucesso: {Nome}", visitanteAutorizado.Nome);
                        // Atualiza a mensagem para indicar que foi excluído
                        mensagemAcesso += " - EXCLUÍDO";
                    }
                    else
                    {
                        _logger.LogWarning("⚠️  Falha ao excluir visitante: {Nome}", visitanteAutorizado.Nome);
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "❌ Erro ao excluir visitante: {Nome}", visitanteAutorizado.Nome);
                    // Não lança exceção para não afetar o fluxo principal
                }
            }

            _logger.LogInformation("🏁 Processamento concluído para placa: {Plate}. Tipo: {Tipo}, Acesso: {Acesso}",
                request.Plate, isVisitante ? "VISITANTE" : "MORADOR", accessResult.ShouldReturn ? "LIBERADO" : "NEGADO");

            _logger.LogInformation("📋 RESUMO DOS PACOTES:");
            if (serialData != null)
            {
                foreach (var data in serialData)
                {
                    _logger.LogInformation("   Canal {Canal}: {Bytes} bytes, Base64: {Base64}",
                        data.serialChannel,
                        data.dataLen,
                        data.data?.Substring(0, Math.Min(20, data.data?.Length ?? 0)) + "...");
                }
            }

            _logger.LogInformation("📨 RETORNANDO PARA ALPHADIGI:");

           


            return new AlarmInfoPlateResponseDTO
            {
                ResponseAlarmInfoPlate = new Response_AlarmInfoPlate
                {
                    info = accessResult.ShouldReturn ? "ok" : "no",
                    serialData = serialData
                }
            };



        }
        catch (Exception e)
        {
            _logger.LogError(e, "Erro em ProcessPlate. Abortando.");
            throw;
        }
    }
}