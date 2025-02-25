using Alphadigi_migration.Models;
using Alphadigi_migration.Data;

    namespace Alphadigi_migration.Services;

    public interface IAccessHandler
    {
        Task<(bool ShouldReturn, string Acesso)> HandleAccessAsync(Veiculo veiculo, Alphadigi alphadigi);
    }

    public class VisitaAccessHandler : IAccessHandler
    {
        public Task<(bool ShouldReturn, string Acesso)> HandleAccessAsync(Veiculo veiculo, Alphadigi alphadigi)
        {
            return Task.FromResult((false, "NÃO CADASTRADO"));
        }
    }
    public class SaidaSempreAbreAccessHandler : IAccessHandler
    {
        private readonly IVeiculoService _veiculoService;

        public SaidaSempreAbreAccessHandler(IVeiculoService veiculoService)
        {
            _veiculoService = veiculoService;
        }
        public async Task<(bool ShouldReturn, string Acesso)> HandleAccessAsync(Veiculo veiculo, Alphadigi alphadigi)
        {
            string acesso;

            if (veiculo.Id != null)
            {
                await _veiculoService.UpdateVagaVeiculo(veiculo.Id, false);
                acesso = "CADASTRADO";
            }
            else
            {
                acesso = "NÃO CADASTRADO";
            }
            return (true, acesso);
        }
    }

    public class ControlaVagaAccessHandler : IAccessHandler
    {
        private readonly IVeiculoService _veiculoService;
        private readonly UnidadeService _unidadeService;

        public ControlaVagaAccessHandler(IVeiculoService veiculoService, UnidadeService unidadeService)
        {
            _veiculoService = veiculoService;
            _unidadeService = unidadeService;
        }
        public async Task<(bool ShouldReturn, string Acesso)> HandleAccessAsync(Veiculo veiculo, Alphadigi alphadigi)
        {
            string acesso;
            bool abre;
            if (!alphadigi.Sentido)
            {
                await _veiculoService.UpdateVagaVeiculo(veiculo.Id, false);
                acesso = "";
                abre = true;
            }
            else
            {
                var vagas = await _unidadeService.GetUnidadeInfo(veiculo.UnidadeNavigation.Id);
                if (vagas.NumVagas > vagas.VagasOcupadasMoradores || veiculo.VeiculoDentro)
                {
                    await _veiculoService.UpdateVagaVeiculo(veiculo.Id, true);
                    acesso = "";
                    abre = true;
                }
                else
                {
                    acesso = "S/VG";
                    abre = false;
                }
            }
            return (abre, acesso);
        }
    }
    public class NaoControlaVagaAccessHandler : IAccessHandler
    {
        private readonly IVeiculoService _veiculoService;
        public NaoControlaVagaAccessHandler(IVeiculoService veiculoService)
        {
            _veiculoService = veiculoService;
        }
        public async Task<(bool ShouldReturn, string Acesso)> HandleAccessAsync(Veiculo veiculo, Alphadigi alphadigi)
        {
            string acesso;
            bool abre;
            if (!alphadigi.Sentido)
            {
                await _veiculoService.UpdateVagaVeiculo(veiculo.Id, false);
                acesso = "";
                abre = true;
            }
            else
            {
                await _veiculoService.UpdateVagaVeiculo(veiculo.Id, true);
                acesso = "";
                abre = true;
            }
            return (abre, acesso);
        }
    }

    public class AccessHandlerFactory
    {
    private readonly AppDbContextSqlite _contextSqlite;
    private readonly AppDbContextFirebird _contextFirebird;
    private readonly IAlphadigiService _alphadigiService;
    private readonly IVeiculoService _veiculoService;
    private readonly UnidadeService _unidadeService;
    private readonly ILogger<AlphadigiHearthBeatService> _logger;
    private readonly MonitorAcessoLinear _monitorAcessoLinear;


    public AccessHandlerFactory(
            AppDbContextSqlite contextSqlite,
            AppDbContextFirebird contextFirebird,
            IAlphadigiService alphadigiService,
            IVeiculoService veiculoService,
            UnidadeService unidadeService,
            MonitorAcessoLinear monitorAcessoLinear,
            ILogger<AlphadigiHearthBeatService> logger) // Adicione o logger
    {
        _contextSqlite = contextSqlite;
        _contextFirebird = contextFirebird;
        _alphadigiService = alphadigiService;
        _veiculoService = veiculoService;
        _unidadeService = unidadeService;
        _monitorAcessoLinear = monitorAcessoLinear;
        _logger = logger; // Salve o logger
    }

        public IAccessHandler GetAccessHandler(Area area)
        {
            if (area.EntradaVisita || area.SaidaVisita)
            {
                return new VisitaAccessHandler();
            }
            if (area.SaidaSempreAbre)
            {
                return new SaidaSempreAbreAccessHandler(_veiculoService);
            }
            if (area.ControlaVaga)
            {
                return new ControlaVagaAccessHandler(_veiculoService, _unidadeService);
            }
            return new NaoControlaVagaAccessHandler(_veiculoService);
        }
    }
