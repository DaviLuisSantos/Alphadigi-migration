using Alphadigi_migration.DTO.PlacaLida;
using Carter;
using Microsoft.Extensions.Logging; // Adicionar using para ILogger
using System.Text.Json;
using Microsoft.AspNetCore.Http; // Adicionar using para IResult e StatusCodes

namespace Alphadigi_migration.Endpoints
{
    public class LogEndpoint : CarterModule
    {
        // Opção 1: Método estático dentro da classe do Endpoint
        public static async Task<IResult> HandleGetDatePlateRequest(
            LogGetDatePlateDTO logPayload,
            IPlacaLidaService logService, // Injetar a INTERFACE
            ILogger<LogEndpoint> logger) // Injetar ILogger
        {
            // Mantenha as opções se precisar delas especificamente para Results.Json
            var options = new JsonSerializerOptions
            {
                PropertyNamingPolicy = null, // Mantenha null se for intencional
                WriteIndented = true
            };

            try
            {
                var resposta = await logService.GetDatePlate(logPayload);

                // Results.Json lida com a serialização, passar options é opcional aqui
                // Se o tipo de 'resposta' for conhecido e simples, pode omitir options
                return Results.Json(resposta, options, statusCode: StatusCodes.Status200OK);
            }
            catch (Exception ex)
            {
                // Use ILogger para registrar o erro
                logger.LogError(ex, "Erro ao processar /Log/getDatePlate para payload: {Plate}", logPayload.Page); // Log mais informativo
                return Results.Problem(
                            title: "Erro interno do servidor.",
                            statusCode: StatusCodes.Status500InternalServerError); // Seja explícito no erro
            }
        }

        public override void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapPost("/Log/getDatePlate", HandleGetDatePlateRequest); // Referencie o método estático
            // O ASP.NET Core automaticamente injetará IPlacaLidaService e ILogger<LogEndpoint> no método
        }
    }
}