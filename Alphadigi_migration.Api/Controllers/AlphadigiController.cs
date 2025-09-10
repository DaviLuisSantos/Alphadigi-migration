using Alphadigi_migration.Api.Factories;
using Alphadigi_migration.Application.Commands.Alphadigi;
using Alphadigi_migration.Domain.DTOs.Alphadigi;
using Alphadigi_migration.Domain.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace Alphadigi_migration.Api.Controllers;

[ApiController]
[Route("LPR")]
public class AlphadigiController : ControllerBase
{
    private readonly ILogger<AlphadigiController> _logger;
    private readonly IMediator _mediator;
    private readonly IHeartbeatFactory _heartbeatFactory;
    private readonly IAlphadigiHearthBeatService _hearthbeatService;

    public AlphadigiController(
        ILogger<AlphadigiController> logger,
        IMediator mediator,
        IHeartbeatFactory heartbeatFactory,
        IAlphadigiHearthBeatService hearthbeatService)
    {
        _logger = logger;
        _mediator = mediator;
        _heartbeatFactory = heartbeatFactory;
        _hearthbeatService = hearthbeatService;
    }

    [HttpGet]
    public async Task<IActionResult> GetVeiculos()
    {
        try
        {
            var query = new Application.Queries.Veiculo.GetVeiculosQuery();
            var result = await _mediator.Send(query);
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting veiculos");
            return StatusCode(500, "Internal server error");
        }
    }

    [HttpPost("placa")]
    public async Task<IActionResult> ProcessPlate([FromBody] AlarmInfoPlateDTO requestBody)
    {
        try
        {
            if (requestBody == null || requestBody.AlarmInfoPlate?.result?.PlateResult == null)
            {
                return BadRequest("Invalid request body");
            }

            var alarm = requestBody.AlarmInfoPlate;
            var result = alarm.result?.PlateResult;

            var command = new ProcessPlateCommand(
                ip: alarm.ipaddr,
                plate: result?.license,
                isRealPlate: result?.realplate ?? false,
                isCad: result?.Whitelist == 2,
                carImage: result?.imageFile,
                plateImage: result?.imageFragmentFile,
                modelo: alarm?.deviceName
            );

            var plateResult = await _mediator.Send(command);

            if (plateResult == null)
            {
                return BadRequest("PlateResult is null");
            }

            var options = new JsonSerializerOptions { PropertyNamingPolicy = null, WriteIndented = true };
            var jsonResult = JsonSerializer.Serialize(plateResult, options);
            await System.IO.File.WriteAllTextAsync("response.json", jsonResult);

            return Ok(plateResult);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing plate");
            return StatusCode(500, "Internal server error");
        }
    }

    [HttpPost("heartbeat")]
    public async Task<IActionResult> ProcessHeartbeat()
    {
        try
        {
            using var reader = new StreamReader(Request.Body);
            var body = await reader.ReadToEndAsync();
            _logger.LogInformation("ProcessHeartbeat chamado com dados: {RequestBody}", body);

            var request = _heartbeatFactory.Create(body);
            if (request == null)
                return BadRequest("Formato de requisição não reconhecido.");

            var ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString();
            if (ipAddress?.StartsWith("::ffff:") == true)
                ipAddress = ipAddress[7..];

            switch (request)
            {
                case HeartbeatDTO dto:
                    var resposta = await _hearthbeatService.ProcessHearthBeat(ipAddress);
                    if (resposta is ResponseHeathbeatDTO)
                    {
                        var options = new JsonSerializerOptions { PropertyNamingPolicy = null, WriteIndented = true };
                        return new JsonResult(resposta, options);
                    }
                    return Ok(resposta);

                case ReturnAddPlateDTO dto:
                    if (dto.Response_AddWhiteList == null)
                        return BadRequest("Response_AddWhiteList não pode ser nulo.");

                    await _hearthbeatService.HandleCreateReturn(ipAddress);
                    return Ok($"Adição processada para: {dto.Response_AddWhiteList.serialno}");

                case ReturnDelPlateDTO dto:
                    if (dto.Response_DelWhiteListAll == null)
                        return BadRequest("Response_DelWhiteListAll não pode ser nulo.");

                    await _hearthbeatService.HandleDeleteReturn(ipAddress);
                    return Ok($"Remoção processada para: {dto.Response_DelWhiteListAll.serialno}");

                default:
                    return BadRequest("Tipo não suportado.");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro no endpoint /LPR/heartbeat");
            return StatusCode(500, "Erro interno do servidor.");
        }
    }

    [HttpPost("create")]
    public async Task<IActionResult> Create([FromBody] string placa)
    {
        try
        {
            if (string.IsNullOrEmpty(placa))
            {
                return BadRequest("Placa cannot be null or empty");
            }

            var command = new UpdateStageCommand(placa);
            await _mediator.Send(command);

            return Ok();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in create operation");
            return StatusCode(500, "Internal server error");
        }
    }

    [HttpPost("delete")]
    public async Task<IActionResult> Delete([FromBody] string placa)
    {
        try
        {
            if (string.IsNullOrEmpty(placa))
            {
                return BadRequest("Placa cannot be null or empty");
            }

            var command = new UpdateStageCommand(placa);
            await _mediator.Send(command);

            return Ok();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in delete operation");
            return StatusCode(500, "Internal server error");
        }
    }
}