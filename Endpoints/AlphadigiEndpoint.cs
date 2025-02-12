using Alphadigi_migration.DTO.Alphadigi;
using Alphadigi_migration.Services;
using Carter;
using Carter.OpenApi;

namespace Alphadigi_migration;

public class AlphadigiEndpoint : CarterModule
{
    public override void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("/", (VeiculoService veiculo) =>
        {
            return veiculo.GetVeiculos();
        })
        .IncludeInOpenApi();

        app.MapPost("/LPR/placa", async (AlarmInfoPlateDTO requestBody, VeiculoService veiculoService) =>
        {

            if (requestBody == null || requestBody.AlarmInfoPlate == null)
            {
                return Results.BadRequest("Invalid request body");
            }

            var plateResult = requestBody.AlarmInfoPlate.result?.PlateResult;
            if (plateResult == null)
            {
                return Results.BadRequest("PlateResult is null");
            }

            return Results.Ok();
        });

        app.MapPost("/LPR/heartbeat", async (HeartbeatDTO requestBody, VeiculoService veiculo) =>
        {
            if (requestBody == null)
            {
                return Results.BadRequest("Invalid request body");
            }
            return Results.Ok();
        });
    }
}
