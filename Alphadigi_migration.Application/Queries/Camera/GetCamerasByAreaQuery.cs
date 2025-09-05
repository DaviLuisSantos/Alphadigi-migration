using MediatR;

namespace Alphadigi_migration.Application.Queries.Camera;

public class GetCamerasByAreaQuery : IRequest<List<Domain.EntitiesNew.Camera>>
{
    public Guid AreaId { get; set; }
}