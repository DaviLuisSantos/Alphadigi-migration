using Alphadigi_migration.Application.Service;
using Alphadigi_migration.Application.Services;
using Alphadigi_migration.Domain.DTOs.Alphadigi;
using Alphadigi_migration.Domain.Interfaces;
using Alphadigi_migration.Factories;
using Carter;
using Carter.OpenApi;
using System.Text.Json;

namespace Alphadigi_migration;

public class AlphadigiEndpoint : CarterModule
{
    public override void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("/", (IVeiculoService veiculo) =>
        {
            return veiculo.GetVeiculos();
        })
        .IncludeInOpenApi();

        app.MapPost("/LPR/placa", async (AlarmInfoPlateDTO requestBody, IAlphadigiPlateService plateService) =>
        {

            if (requestBody == null || requestBody.AlarmInfoPlate == null)
            {
                return Results.BadRequest("Invalid request body");
            }

            var alarm = requestBody.AlarmInfoPlate;
            var result = alarm.result?.PlateResult;

            var placa = new ProcessPlateDTO
            {
                ip = alarm.ipaddr,
                plate = result?.license,
                isRealPlate = result?.realplate ?? false,
                isCad = result?.Whitelist == 2,
                carImage = result?.imageFile,
                plateImage = result?.imageFragmentFile,
                modelo = alarm?.deviceName
            };


            var plateResult = await plateService.ProcessPlate(placa);
            if (plateResult == null)
            {
                return Results.BadRequest("PlateResult is null");
            }
            var options = new JsonSerializerOptions
            {
                PropertyNamingPolicy = null,
                WriteIndented = true
            };
            var filePath = "response.json"; // Defina o caminho do arquivo
            var jsonResult = JsonSerializer.Serialize(plateResult, options);
            await File.WriteAllTextAsync(filePath, jsonResult);
            return Results.Json(plateResult, options);

        });

        app.MapPost("/LPR/heartbeat", async (HttpContext context, IHeartbeatFactory factory, IAlphadigiHearthBeatService hearthbeatService) =>
        {
            try
            {
                using var reader = new StreamReader(context.Request.Body);
                var body = await reader.ReadToEndAsync();

                var request = factory.Create(body);
                if (request == null)
                    return Results.BadRequest("Formato de requisição não reconhecido.");

                var ipAddress = context.Connection.RemoteIpAddress?.ToString();
                if (ipAddress?.StartsWith("::ffff:") == true)
                    ipAddress = ipAddress[7..];

                switch (request)
                {
                    case HeartbeatDTO dto:
                        var resposta = await hearthbeatService.ProcessHearthBeat(ipAddress);
                        if (resposta is ResponseHeathbeatDTO)
                        {
                            var options = new JsonSerializerOptions
                            {
                                PropertyNamingPolicy = null,
                                WriteIndented = true
                            };
                            return Results.Json(resposta, options);
                        }
                        return Results.Json(resposta);

                    case ReturnAddPlateDTO dto:
                        // Lógica para Response_AddWhiteList
                        if (dto.Response_AddWhiteList == null)
                            return Results.BadRequest("Response_AddWhiteList não pode ser nulo.");
                        await hearthbeatService.HandleCreateReturn(ipAddress);
                        return Results.Ok($"Adição processada para: {dto.Response_AddWhiteList.serialno}");

                    case ReturnDelPlateDTO dto:
                        // Lógica para Response_DelWhiteListAll
                        if (dto.Response_DelWhiteListAll == null)
                            return Results.BadRequest("Response_DelWhiteListAll não pode ser nulo.");
                        await hearthbeatService.HandleDeleteReturn(ipAddress);
                        return Results.Ok($"Remoção processada para: {dto.Response_DelWhiteListAll.serialno}");

                    default:
                        return Results.BadRequest("Tipo não suportado.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro no endpoint /LPR/heartbeat: {ex}");
                return Results.Problem("Erro interno do servidor.");
            }
        });

        app.MapPost("/LPR/create", async (HttpContext context, string placa, IAlphadigiService alphadigiService) =>
        {
            if (placa == null)
            {
                return Results.BadRequest("Invalid request body");
            }
            await alphadigiService.UpdateStage("SEND");

            return Results.Ok();
        });

        app.MapPost("/LPR/delete", async (HttpContext context, string placa, IAlphadigiService alphadigiService) =>
        {
            if (placa == null)
            {
                return Results.BadRequest("Invalid request body");
            }
            await alphadigiService.UpdateStage("SEND");

            return Results.Ok();
        });
    }
}
