using MediatR;


namespace Alphadigi_migration.Application.Commands.MensagemDisplay;

public class SaveMensagemDisplayCommand : IRequest<bool>
{
    public string Placa { get; set; }
    public string Mensagem { get; set; }
    public int AlphadigiId { get; set; }
    public DateTime DataHora { get; set; }
    public int Prioridade { get; set; } = 1;
}