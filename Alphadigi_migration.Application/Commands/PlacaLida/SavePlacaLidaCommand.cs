

using MediatR;

namespace Alphadigi_migration.Application.Commands.PlacaLida;

public class SavePlacaLidaCommand : IRequest<bool>
{
    public string Placa { get; set; }
    public Guid AlphadigiId { get; set; }
    public int AreaId { get; set; }
    public DateTime DataHora { get; set; }
    public string CarroImg { get; set; }
    public string PlacaImg { get; set; }
    public bool Real { get; set; } = true;
    public bool Cadastrado { get; set; } = false;
}