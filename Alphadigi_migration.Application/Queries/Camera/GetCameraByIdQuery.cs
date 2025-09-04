using MediatR;

namespace Alphadigi_migration.Application.Queries.Camera;

public class GetCameraByIdQuery : IRequest<Domain.EntitiesNew.Camera>
{
    public Guid Id { get; set; }
}