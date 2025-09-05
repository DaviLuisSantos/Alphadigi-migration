using Alphadigi_migration.Domain.DTOs.Camera;
using MediatR;

namespace Alphadigi_migration.Application.Commands.Camera;

public class CreateCameraCommand : IRequest<Guid>
{
    public string Nome { get; set; }
    public string Ip { get; set; }
    public Guid IdArea { get; set; }
    public string Modelo { get; set; }
    public string Direcao { get; set; }
    public bool FotoEvento { get; set; }

    public CreateCameraCommand(CreateCameraDTO dto)
    {
        Nome = dto.Nome;
        Ip = dto.Ip;
        IdArea = dto.IdArea;
        Modelo = dto.Modelo;
        Direcao = dto.Direcao;
        FotoEvento = dto.FotoEvento;
    }
}