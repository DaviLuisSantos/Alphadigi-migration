using MediatR;

namespace Alphadigi_migration.Application.Queries.Camera;

public class GetCameraByIdQuery : IRequest<Domain.EntitiesNew.Camera>
{
    public int Id { get; set; }
}