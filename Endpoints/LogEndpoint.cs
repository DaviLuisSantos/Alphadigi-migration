using Alphadigi_migration.DTO.PlacaLida;
using Alphadigi_migration.Services;
using Carter;
using System.Text.Json;

namespace Alphadigi_migration.Endpoints;

public class LogEndpoint : CarterModule
{
    public override void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPost("/Log/getDatePlate", async (LogGetDatePlateDTO logPayload, PlacaLidaService logService) =>
        {
            var options = new JsonSerializerOptions
            {
                PropertyNamingPolicy = null,
                WriteIndented = true
            };
            try
            {  

                var resposta = await logService.GetDatePlate(logPayload);

                var jsonResult = JsonSerializer.Serialize(resposta, options);

                return Results.Json(resposta, options);
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
