using Alphadigi_migration.Application.Queries.Alphadigi;
using Alphadigi_migration.Application.Service;
using Alphadigi_migration.Domain.DTOs.Alphadigi;
using MediatR;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/test")]
public class TestController : ControllerBase
{
    private readonly DisplayService _displayService;
    private readonly IMediator _mediator;

    [HttpPost("testar-display")]
    public async Task<IActionResult> TestarDisplay([FromBody] TestDisplayRequest request)
    {
        try
        {
            // Buscar câmera
            var camera = await _mediator.Send(new GetOrCreateAlphadigiQuery { Ip = request.Ip });

            // Gerar dados
            var serialData = await _displayService.RecieveMessageAlphadigi(
                request.Placa,
                request.Acesso,
                camera);

            // Mostrar detalhes
            var detalhes = serialData?.Select(s => new
            {
                canal = s.serialChannel,
                base64 = s.data?.Substring(0, Math.Min(30, s.data?.Length ?? 0)) + "...",
                tamanho = s.dataLen,
                tamanhoReal = s.data?.Length ?? 0
            }).ToList();

            return Ok(new
            {
                placa = request.Placa,
                camera = camera.Ip,
                linhasDisplay = camera.LinhasDisplay,
                pacotes = serialData?.Count ?? 0,
                detalhes,
                respostaCompleta = new
                {
                    Response_AlarmInfoPlate = new
                    {
                        info = "ok",
                        serialData = serialData
                    }
                }
            });
        }
        catch (Exception ex)
        {
            return BadRequest(new { erro = ex.Message });
        }
    }

    [HttpGet("test-fixed-package")]
    public IActionResult TestFixedPackage()
    {
        // Pacote Base64 que funcionava na versão antiga para "LIBERADO"
        var knownWorkingPackage = "AGT//244AAIAAAAKAP//AAAHS1hPOUcxMQ0BAAAKAAD/AAAITElCRVJBRE8AHh8AAAAAAAAAAAAAAAAAAAAAAAA=";

        // Pacote do sinal serial (sempre o mesmo)
        var sinalSerial = "AGT//w8GAAEAAAAFJLc=";

        return Ok(new
        {
            message = "Use estes pacotes para testar diretamente na câmera",
            packages = new[]
            {
            new {
                channel = 0,
                data = knownWorkingPackage,
                dataLen = 65,
                description = "Pacote principal (placa + LIBERADO)"
            },
            new {
                channel = 0,
                data = sinalSerial,
                dataLen = 14,
                description = "Sinal serial (para CADASTRADO)"
            }
        }
        });
    }

  
}

public class TestDisplayRequest
{
    public string Placa { get; set; } = "TESTE123";
    public string Acesso { get; set; } = "CADASTRADO";
    public string Ip { get; set; } = "192.168.0.182";
}