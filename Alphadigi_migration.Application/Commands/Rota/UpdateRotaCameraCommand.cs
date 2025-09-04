using MediatR;

namespace Alphadigi_migration.Application.Commands.Rota;

public class UpdateRotaCameraCommand : IRequest
{
    public Guid Id { get; set; }
    public int NovaCameraId { get; set; }

    public UpdateRotaCameraCommand(Guid id, int novaCameraId)
    {
        Id = id;
        NovaCameraId = novaCameraId;
    }
}
