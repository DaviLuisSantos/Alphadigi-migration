using Alphadigi_migration.Data;
using Alphadigi_migration.Models;
using Alphadigi_migration.DTO.Alphadigi;

namespace Alphadigi_migration.Services
{
    public class AlphadigiHearthBeatService : IAlphadigiHearthBeatService
    {
        private readonly AppDbContextSqlite _contextSqlite;
        private readonly AppDbContextFirebird _contextFirebird;
        private readonly IAlphadigiService _alphadigiService;
        private readonly IVeiculoService _veiculoService;
        private readonly ILogger<AlphadigiHearthBeatService> _logger;

        public AlphadigiHearthBeatService(
            AppDbContextSqlite contextSqlite,
            AppDbContextFirebird contextFirebird,
            IAlphadigiService alphadigiService,
            IVeiculoService veiculoService,
            ILogger<AlphadigiHearthBeatService> logger) // Adicione o logger
        {
            _contextSqlite = contextSqlite;
            _contextFirebird = contextFirebird;
            _alphadigiService = alphadigiService;
            _veiculoService = veiculoService;
            _logger = logger; // Salve o logger
        }

        public async Task<object> ProcessHearthBeat(string ip)
        {
            _logger.LogInformation($"ProcessHearthBeat chamado com IP: {ip}"); // Log do parâmetro
            try
            {
                var alphadigi = await _alphadigiService.GetOrCreate(ip);
                var hearthBeat = await HandleAlphadigiStage(alphadigi);

                return hearthBeat;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro em ProcessHearthBeat");
                throw; // Relance a exceção para que ela seja tratada em um nível superior
            }
        }

        public async Task<object> HandleAlphadigiStage(Alphadigi alphadigi)
        {
            string stage = alphadigi.Estado ?? "DELETE";
            string newStage = null;
            object response = null;

            switch (stage)
            {
                case "DELETE":
                    response = HandleDelete(alphadigi);
                    newStage = "CREATE";
                    break;
                case "CREATE":
                    response = await HandleCreate(alphadigi);
                    newStage = "SEND";
                    break;
                case "SEND":
                    response = await HandleCreate(alphadigi);
                    newStage = response == null ? "FINAL" : "SEND";
                    break;
                default:
                    return null;
            }

            alphadigi.Estado = newStage;
            _contextSqlite.Update(alphadigi);
            await _contextSqlite.SaveChangesAsync();
            return response;
        }

        public DeleteWhiteListAllDTO HandleDelete(Alphadigi alphadigi)
        {
            return new DeleteWhiteListAllDTO
            {
                DeleteWhiteListAll = 1
            };
        }

        public async Task<AddWhiteListDTO> HandleCreate(Alphadigi alphadigi)
        {
            int ultimoId = alphadigi.UltimoId ?? 0;
            var veiculosEnvio = await _veiculoService.GetVeiculosSend(ultimoId);

            if (veiculosEnvio.Count == 0)
            {
                return null;
            }

            // Atualiza o UltimoId com o ID do último veículo enviado
            alphadigi.UltimoId = veiculosEnvio.Max(item => item.Id);

            var envio = new AddWhiteListDTO
            {
                AddWhiteList = new AddWhiteList
                {
                    add_data = veiculosEnvio.Select(item => new AddData
                    {
                        Carnum = item.Placa,
                        Startime = "20200718155220",
                        Endtime = "30990718155220"
                    }).ToList()
                }
            };

            // Salva as mudanças no banco de dados
            _contextSqlite.Update(alphadigi);
            await _contextSqlite.SaveChangesAsync();

            return envio;
        }
    }
}
