

using MediatR;

namespace Alphadigi_migration.Application.Commands.Camera;

public class AtivarCameraCommand : IRequest<bool>
{
    public int Id { get; set; }
}