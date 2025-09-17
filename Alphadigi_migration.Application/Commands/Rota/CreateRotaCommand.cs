using MediatR;

namespace Alphadigi_migration.Application.Commands.Rota;

public class CreateRotaCommand : IRequest<Guid>
{
    public int RotaId { get; set; }
    public int CameraId { get; set; }

    public CreateRotaCommand(int rotaId, int cameraId)
    {
        RotaId = rotaId;
        CameraId = cameraId;
    }
}
