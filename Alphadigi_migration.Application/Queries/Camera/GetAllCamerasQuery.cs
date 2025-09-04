using MediatR;

namespace Alphadigi_migration.Application.Queries.Camera;

public class GetAllCamerasQuery : IRequest<List<Domain.EntitiesNew.Camera>>
{
}