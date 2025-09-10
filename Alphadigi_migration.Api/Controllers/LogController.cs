using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Text.Json;
using Alphadigi_migration.Domain.Interfaces;
using Alphadigi_migration.Domain.DTOs.PlacaLidas;

namespace Alphadigi_migration.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class LogController : ControllerBase
    {
        private readonly IPlacaLidaService _logService;
        private readonly ILogger<LogController> _logger;

        public LogController(IPlacaLidaService logService, ILogger<LogController> logger)
        {
            _logService = logService;
            _logger = logger;
        }

        [HttpPost("getDatePlate")]
        public async Task<IActionResult> GetDatePlate([FromBody] LogGetDatePlateDTO logPayload)
        {
            if (logPayload == null)
                return BadRequest("Payload inválido");

            var options = new JsonSerializerOptions
            {
                PropertyNamingPolicy = null,
                WriteIndented = true
            };

            try
            {
                var resposta = await _logService.GetDatePlate(logPayload);
                return new JsonResult(resposta, options) { StatusCode = 200 };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao processar /Log/getDatePlate para payload: {Payload}", logPayload);
                return StatusCode(500, "Erro interno do servidor");
            }
        }
    }
}
