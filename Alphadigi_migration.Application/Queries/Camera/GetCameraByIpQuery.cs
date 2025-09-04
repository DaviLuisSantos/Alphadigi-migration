using MediatR;

namespace Alphadigi_migration.Application.Queries.Camera;

public class GetCameraByIpQuery : IRequest<Domain.EntitiesNew.Camera>
{
    public string Ip { get; set; }
}
