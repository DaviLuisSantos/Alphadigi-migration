using Alphadigi_migration.Domain.DTOs.Alphadigi;
using MediatR;

namespace Alphadigi_migration.Application.Commands.Display;

public class CreateHeartbeatDisplayPackageCommand : IRequest<List<SerialData>>
{
    public string Placa { get; set; }
    public string Acesso { get; set; }
    public int AlphadigiId { get; set; }

    public CreateHeartbeatDisplayPackageCommand(string placa, string acesso, int alphadigiId)
    {
        Placa = placa;
        Acesso = acesso;
        AlphadigiId = alphadigiId;
    }
}
