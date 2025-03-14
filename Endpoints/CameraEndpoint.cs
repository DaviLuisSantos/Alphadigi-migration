using Alphadigi_migration.DTO.Alphadigi;
using Alphadigi_migration.Models;
using Alphadigi_migration.Services;
using Carter;
using Carter.OpenApi;
using System.Text.Json;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace Alphadigi_migration;

public class CameraEndpoint : CarterModule
{
    public override void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("/camera/", async (IAlphadigiService cameras) =>
        {
            return await cameras.GetAll();
        })
        .IncludeInOpenApi();

        app.MapPost("/camera/create", async (CreateAlphadigiDTO alphadigi, IAlphadigiService camera) =>
        {
            return await camera.Create(alphadigi);
        })
        .IncludeInOpenApi();

        app.MapPut("/camera/update", async (Alphadigi alphadigi, IAlphadigiService cameras) =>
        {
            return await cameras.Update(alphadigi);
        })
        .IncludeInOpenApi();

        app.MapDelete("/camera/delete", async (int id, IAlphadigiService cameras) =>
        {
            return await cameras.Delete(id);
        })
        .IncludeInOpenApi();
    }
}
