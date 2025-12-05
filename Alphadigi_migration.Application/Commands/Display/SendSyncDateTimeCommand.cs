using MediatR;

namespace Alphadigi_migration.Application.Commands.Display;

public class SyncDateTimeCommand : IRequest
{
    public string IpTotem { get; set; }
    public int Porta { get; set; } = 5000;
}