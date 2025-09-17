using Alphadigi_migration.Domain.DTOs.Alphadigi;
using MediatR;


namespace Alphadigi_migration.Application.Commands.Display;

public class CreateDisplayPackageCommand : IRequest<List<SerialData>>
{
    public string Placa { get; set; }
    public string Acesso { get; set; }
    public int AlphadigiId { get; set; }

    public CreateDisplayPackageCommand(string placa, string acesso, int alphadigiId)
    {
        Placa = placa;
        Acesso = acesso;
        AlphadigiId = alphadigiId;
    }
}
