using MediatR;

namespace Alphadigi_migration.Application.Commands.Camera;

public class DesativarCameraCommand : IRequest<bool>
{
    public Guid Id { get; set; }
    public string Motivo { get; set; }
}
