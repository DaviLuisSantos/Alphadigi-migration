using MediatR;


namespace Alphadigi_migration.Application.Commands.Camera;

public class DeleteCameraCommand : IRequest<bool>
{
    public Guid Id { get; set; }

    public DeleteCameraCommand(Guid id)
    {
        Id = id;
    }
}
