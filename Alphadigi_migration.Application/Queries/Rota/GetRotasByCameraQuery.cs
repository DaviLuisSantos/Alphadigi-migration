using Alphadigi_migration.Application.DTOs.Rota;
using MediatR;

namespace Alphadigi_migration.Application.Queries.Rota;

public class GetRotasByCameraQuery : IRequest<IEnumerable<RotaDto>>
{
    public int CameraId { get; set; }

    public GetRotasByCameraQuery(int cameraId)
    {
        CameraId = cameraId;
    }
}