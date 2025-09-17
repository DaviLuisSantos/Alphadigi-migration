using MediatR;

namespace Alphadigi_migration.Application.Queries.Camera;

public class GetCamerasByAreaQuery : IRequest<List<Domain.EntitiesNew.Camera>>
{
    public int AreaId { get; set; }
}