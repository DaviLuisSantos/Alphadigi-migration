using Alphadigi_migration.DTO.Alphadigi;
using Alphadigi_migration.Services;
using Carter;
using System.Text.Json;

namespace Alphadigi_migration.Endpoints;

public class LogEndpoint : CarterModule
{
    public override void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPost("/Log/heartbeat", async (HttpContext context, HeartbeatDTO requestBody, IAlphadigiHearthBeatService hearthbeatService) =>
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
    }
}
