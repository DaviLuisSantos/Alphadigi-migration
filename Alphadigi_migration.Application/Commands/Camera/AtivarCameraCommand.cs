

using MediatR;

namespace Alphadigi_migration.Application.Commands.Camera;

public class AtivarCameraCommand : IRequest<bool>
{
    public Guid Id { get; set; }
}