using Alphadigi_migration.DTO.Alphadigi;
using Alphadigi_migration.Services;
using Carter;
using Carter.OpenApi;
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

            var placa = new ProcessPlateDTO
            {  
                ip = alarm.ipaddr,
                plate = alarm.result?.PlateResult?.license,
                isRealPlate = alarm.result?.PlateResult?.realplate ?? false,
                isCad = alarm.result?.PlateResult.Whitelist == 2
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

            var jsonResult = System.Text.Json.JsonSerializer.Serialize(plateResult, options);

            // Salva o JSON em um arquivo
            var filePath = "response.json"; // Defina o caminho do arquivo
            await File.WriteAllTextAsync(filePath, jsonResult);

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
                var resposta = await hearthbeatService.ProcessHearthBeat(ipAddress); //Aguarde aqui
                return Results.Ok(resposta); //Retorne os resultados do método corretamente.
            }
            catch (Exception ex)
            {
                // Log the exception
                Console.WriteLine($"Erro no endpoint /LPR/heartbeat: {ex}");
                return Results.Problem("Erro interno do servidor."); //Retorne um erro adequado.
            }
        });
    }
}
