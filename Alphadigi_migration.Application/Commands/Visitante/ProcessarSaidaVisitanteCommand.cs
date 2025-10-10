using MediatR;

namespace Alphadigi_migration.Application.Commands.Visitante;

public class ProcessarSaidaVisitanteCommand : IRequest<bool>
{
    public string Placa { get; set; }
    public int CameraId { get; set; }
    public string IpCamera { get; set; }

    public ProcessarSaidaVisitanteCommand(string placa, int cameraId, string ipCamera)
    {
        Placa = placa;
        CameraId = cameraId;
        IpCamera = ipCamera;
    }
}