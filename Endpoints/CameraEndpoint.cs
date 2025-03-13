using Alphadigi_migration.DTO.Alphadigi;
using Alphadigi_migration.Services;
using Carter;
using Carter.OpenApi;
using System.Text.Json;

namespace Alphadigi_migration;

public class CameraEndpoint : CarterModule
{
    public override void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("/cameras", (IAlphadigiService cameras) =>
        {
            return cameras.GetAll();
        })
        .IncludeInOpenApi();
    }
}
