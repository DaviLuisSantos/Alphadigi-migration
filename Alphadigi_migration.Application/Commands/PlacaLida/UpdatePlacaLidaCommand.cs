using MediatR;


namespace Alphadigi_migration.Application.Commands.PlacaLida;

public class UpdatePlacaLidaCommand : IRequest<bool>
{
    public Guid Id { get; set; }
    public bool? Liberado { get; set; }
    public string Acesso { get; set; }
    public bool? Cadastrado { get; set; }
    public bool? Processado { get; set; }
    public string CarroImg { get; set; }
    public string PlacaImg { get; set; }
    public bool? Real { get; set; }
}