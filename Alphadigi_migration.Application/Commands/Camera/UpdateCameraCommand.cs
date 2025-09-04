
using MediatR;

namespace Alphadigi_migration.Application.Commands.Camera;

public class UpdateCameraCommand : IRequest<bool>
{
    public Guid Id { get; set; }
    public string Nome { get; set; }
    public string Ip { get; set; }
    public Guid IdArea { get; set; }
    public string Modelo { get; set; }
    public string Direcao { get; set; }
    public bool FotoEvento { get; set; }
}