using MediatR;

namespace Alphadigi_migration.Application.Commands.Camera;

public class DesativarCameraCommand : IRequest<bool>
{
    public int Id { get; set; }
    public string Motivo { get; set; }
}
