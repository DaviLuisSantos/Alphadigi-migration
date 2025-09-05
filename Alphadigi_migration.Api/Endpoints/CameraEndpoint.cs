using Alphadigi_migration.Domain.DTOs.Alphadigi;
using Alphadigi_migration.Domain.Interfaces;
using Carter;
using Carter.OpenApi;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace Alphadigi_migration.Api.Endpoints;

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

        app.MapPut("/camera/update", async (UpdateAlphadigiDTO alphadigi, IAlphadigiService cameras) =>
        {
            var res = await cameras.Update(alphadigi);

            return res ? Results.Ok() : Results.InternalServerError("Erro ao atualizar");
        })
        .IncludeInOpenApi();

        app.MapDelete("/camera/delete", async (int id, IAlphadigiService cameras) =>
        {
            return await cameras.Delete(id);
        })
        .IncludeInOpenApi();
    }
}
