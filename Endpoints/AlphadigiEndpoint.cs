using Alphadigi_migration.DTO.Alphadigi;
using Alphadigi_migration.Services;
using Carter;
using Carter.OpenApi;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.IO;
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
                plateImage = result?.imageFragmentFile
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

            return Results.Json(plateResult, options);

        });

        app.MapPost("/LPR/heartbeat", async (HttpContext context, HeartbeatDTO requestBody, IAlphadigiHearthBeatService hearthbeatService) =>
        {
            if (requestBody == null)
            {
                return Results.BadRequest("Invalid request body");
            }

            var ipAddress = context.Connection.RemoteIpAddress?.ToString();
            if (ipAddress != null && ipAddress.StartsWith("::ffff:"))
            {
                ipAddress = ipAddress.Substring(7);
            }

            try
            {
                var options = new JsonSerializerOptions
                {
                    PropertyNamingPolicy = null,
                    WriteIndented = true
                };

                var resposta = await hearthbeatService.ProcessHearthBeat(ipAddress);

                var jsonResult = JsonSerializer.Serialize(resposta, options);

                var filePath = "responseHb.json"; // Defina o caminho do arquivo
                await File.WriteAllTextAsync(filePath, jsonResult);

                return Results.Json(resposta, options);
            }
            catch (Exception ex)
            {
                // Log the exception
                Console.WriteLine($"Erro no endpoint /LPR/heartbeat: {ex}");
                return Results.Problem("Erro interno do servidor."); //Retorne um erro adequado.
            }
        });

        app.MapPost("/LPR/create", async (HttpContext context, string placa, IAlphadigiService alphadigiService) =>
        {
            if (placa == null)
            {
                return Results.BadRequest("Invalid request body");
            }
            await alphadigiService.updateStage("SEND");

            return Results.Ok();
        });
    }
}
