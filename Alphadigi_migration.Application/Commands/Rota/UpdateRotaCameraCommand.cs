using MediatR;

namespace Alphadigi_migration.Application.Commands.Rota;

public class UpdateRotaCameraCommand : IRequest
{
    public int Id { get; set; }
    public int NovaCameraId { get; set; }

    public UpdateRotaCameraCommand(int id, int novaCameraId)
    {
        Id = id;
        NovaCameraId = novaCameraId;
    }
}
