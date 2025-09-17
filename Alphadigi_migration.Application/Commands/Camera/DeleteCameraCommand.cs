using MediatR;


namespace Alphadigi_migration.Application.Commands.Camera;

public class DeleteCameraCommand : IRequest<bool>
{
    public int Id { get; set; }

    public DeleteCameraCommand(int id)
    {
        Id = id;
    }
}
