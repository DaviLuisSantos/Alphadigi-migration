using Alphadigi_migration.Domain.DTOs.Alphadigi;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using Alphadigi_migration.Application.Commands.Alphadigi;

namespace Alphadigi_migration.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AlphadigiController : ControllerBase
{
    private readonly ILogger<AlphadigiController> _logger;
    private readonly IMediator _mediator;

    public AlphadigiController(ILogger<AlphadigiController> logger, IMediator mediator)
    {
        _logger = logger;
        _mediator = mediator;
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

    [HttpPost("LPR/placa")]
    public async Task<IActionResult> ProcessPlate([FromBody] AlarmInfoPlateDTO requestBody)
    {
        try
        {
            if (requestBody == null || requestBody.AlarmInfoPlate == null)
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

            // Log para debug (opcional)
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

    [HttpPost("LPR/heartbeat")]
    public async Task<IActionResult> ProcessHeartbeat()
    {
        try
        {
            using var reader = new StreamReader(Request.Body);
            var body = await reader.ReadToEndAsync();

            var ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString();
            if (ipAddress?.StartsWith("::ffff:") == true)
                ipAddress = ipAddress[7..];

            var command = new ProcessHeartbeatCommand(body, ipAddress);
            var result = await _mediator.Send(command);

            return (IActionResult)result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing heartbeat");
            return StatusCode(500, "Internal server error");
        }
    }

    [HttpPost("LPR/create")]
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

    [HttpPost("LPR/delete")]
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