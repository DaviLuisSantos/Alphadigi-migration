using Alphadigi_migration.Data;
using Alphadigi_migration.Models;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace Alphadigi_migration.Services
{
    public interface IAccessHandler
    {
        Task<(bool ShouldReturn, string Acesso)> HandleAccessAsync(Veiculo veiculo, Alphadigi alphadigi);
    }

    public class VisitaAccessHandler : IAccessHandler
    {
        private readonly ILogger<VisitaAccessHandler> _logger;

        public VisitaAccessHandler(ILogger<VisitaAccessHandler> logger)
        {
            _logger = logger;
        }

        public Task<(bool ShouldReturn, string Acesso)> HandleAccessAsync(Veiculo veiculo, Alphadigi alphadigi)
        {
            _logger.LogInformation($"Processando acesso de visitante para veículo com placa {veiculo?.Placa ?? "Visitante"}.");
            return Task.FromResult((false, "NÃO CADASTRADO"));
        }
    }

    public class SaidaSempreAbreAccessHandler : IAccessHandler
    {
        private readonly IVeiculoService _veiculoService;
        private readonly ILogger<SaidaSempreAbreAccessHandler> _logger;

        public SaidaSempreAbreAccessHandler(IVeiculoService veiculoService, ILogger<SaidaSempreAbreAccessHandler> logger)
        {
            _veiculoService = veiculoService;
            _logger = logger;
        }

        public async Task<(bool ShouldReturn, string Acesso)> HandleAccessAsync(Veiculo veiculo, Alphadigi alphadigi)
        {
            _logger.LogInformation($"Iniciando HandleAccessAsync");
            try
            {
                string acesso = "NÃO CADASTRADO";
                if (veiculo != null && veiculo.Id != 0)
                {
                    _logger.LogInformation($"Aprovando saída do veículo cadastrado com ID {veiculo.Id}.");
                    await _veiculoService.UpdateVagaVeiculo(veiculo.Id, false);
                    acesso = "CADASTRADO";
                }
                else
                {
                    _logger.LogInformation("Aprovando saída para veículo não cadastrado.");
                }
                return (true, acesso);
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"Erro em  HandleAccessAsync. - SaidaSempreAbreAccessHandler");
                throw;
            }
        }
    }

    public class ControlaVagaAccessHandler : IAccessHandler
    {
        private readonly IVeiculoService _veiculoService;
        private readonly IUnidadeService _unidadeService;
        private readonly ILogger<ControlaVagaAccessHandler> _logger;

        public ControlaVagaAccessHandler(IVeiculoService veiculoService, IUnidadeService unidadeService, ILogger<ControlaVagaAccessHandler> logger)
        {
            _veiculoService = veiculoService;
            _unidadeService = unidadeService;
            _logger = logger;
        }

        public async Task<(bool ShouldReturn, string Acesso)> HandleAccessAsync(Veiculo veiculo, Alphadigi alphadigi)
        {
            _logger.LogInformation($"Iniciando HandleAccessAsync");
            try
            {
                _logger.LogInformation($"Gerenciando controle de vaga para veículo com ID {veiculo?.Id ?? 0}, Sentido: {alphadigi.Sentido}.");
                string acesso = "";
                bool abre = true;

                if (!alphadigi.Sentido) // Saída
                {
                    if (veiculo != null)
                    {
                        await _veiculoService.UpdateVagaVeiculo(veiculo.Id, false);
                        _logger.LogInformation($"Liberando vaga para veículo com ID {veiculo.Id}.");
                    }
                    acesso = "";
                }
                else // Entrada
                {
                    if (veiculo != null && veiculo.UnidadeNavigation != null)
                    {
                        var vagas = await _unidadeService.GetUnidadeInfo(veiculo.UnidadeNavigation.Id);
                        if (vagas != null && (vagas.NumVagas > vagas.VagasOcupadasMoradores || veiculo.VeiculoDentro))
                        {
                            await _veiculoService.UpdateVagaVeiculo(veiculo.Id, true);
                            _logger.LogInformation($"Concedendo acesso e ocupando vaga para veículo com ID {veiculo.Id}.");
                            acesso = "";
                        }
                        else
                        {
                            acesso = "S/VG";
                            abre = false;
                        }
                    }
                    else
                    {
                        _logger.LogWarning($"Não foi possível obter informações da unidade para veículo com ID {veiculo?.Id ?? 0}. Acesso negado");
                        acesso = "S/VG";
                        abre = false;
                    }
                }

                return (abre, acesso);
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"Erro em  HandleAccessAsync. - ControlaVagaAccessHandler");
                throw;
            }
        }
    }

    public class NaoControlaVagaAccessHandler : IAccessHandler
    {
        private readonly IVeiculoService _veiculoService;
        private readonly ILogger<NaoControlaVagaAccessHandler> _logger;

        public NaoControlaVagaAccessHandler(IVeiculoService veiculoService, ILogger<NaoControlaVagaAccessHandler> logger)
        {
            _veiculoService = veiculoService;
            _logger = logger;
        }

        public async Task<(bool ShouldReturn, string Acesso)> HandleAccessAsync(Veiculo veiculo, Alphadigi alphadigi)
        {
            _logger.LogInformation($"Iniciando HandleAccessAsync");
            try
            {
                _logger.LogInformation($"Acesso sem controle de vaga para veículo com ID {veiculo?.Id ?? 0}, Sentido: {alphadigi.Sentido}.");

                string acesso = "CADASTRADO";
                bool abre = true;
                if (veiculo != null)
                {
                    if (!alphadigi.Sentido) // Saída
                    {
                        await _veiculoService.UpdateVagaVeiculo(veiculo.Id, false);
                        _logger.LogInformation($"Registrando saída para veículo com ID {veiculo.Id}.");
                    }
                    else // Entrada
                    {
                        await _veiculoService.UpdateVagaVeiculo(veiculo.Id, true);
                        _logger.LogInformation($"Registrando entrada para veículo com ID {veiculo.Id}.");
                    }
                }
                else
                {
                    _logger.LogWarning($"Acesso negado: Veículo não encontrado");
                    acesso = "NÃO CADASTRADO";
                    abre = false;
                }

                return (abre, acesso);
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"Erro em  HandleAccessAsync. - NaoControlaVagaAccessHandler");
                throw;
            }
        }
    }

    public interface IAccessHandlerFactory
    {
        IAccessHandler GetAccessHandler(Area area);
    }

    public class AccessHandlerFactory : IAccessHandlerFactory
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<AccessHandlerFactory> _logger;

        public AccessHandlerFactory(IServiceProvider serviceProvider, ILogger<AccessHandlerFactory> logger)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        public IAccessHandler GetAccessHandler(Area area)
        {
            _logger.LogInformation($"Iniciando GetAccessHandler");
            try
            {
                if (area == null)
                {
                    _logger.LogWarning($"Área nula! Retornando VisitaAccessHandler");
                    return _serviceProvider.GetRequiredService<VisitaAccessHandler>();
                }
                if (area.EntradaVisita || area.SaidaVisita)
                {
                    _logger.LogInformation($"Retornando VisitaAccessHandler");
                    return _serviceProvider.GetRequiredService<VisitaAccessHandler>();
                }
                if (area.SaidaSempreAbre)
                {
                    _logger.LogInformation($"Retornando SaidaSempreAbreAccessHandler");
                    return _serviceProvider.GetRequiredService<SaidaSempreAbreAccessHandler>();
                }
                if (area.ControlaVaga)
                {
                    _logger.LogInformation($"Retornando ControlaVagaAccessHandler");
                    return _serviceProvider.GetRequiredService<ControlaVagaAccessHandler>();
                }
                _logger.LogInformation($"Retornando NaoControlaVagaAccessHandler");
                return _serviceProvider.GetRequiredService<NaoControlaVagaAccessHandler>();
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"Erro em  GetAccessHandler.");
                throw;
            }
        }
    }
}