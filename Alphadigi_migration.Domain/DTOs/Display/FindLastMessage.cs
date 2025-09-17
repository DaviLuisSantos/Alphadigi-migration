namespace Alphadigi_migration.Domain.DTOs.Display;

public class FindLastMessage
{
    public string Placa { get; set; }
    public string Mensagem { get; set; }
    public int AlphadigiId { get; set; }
    public FindLastMessage() { }

    public FindLastMessage(string placa, string mensagem, int alphadigiId)
    {
        Placa = placa;
        Mensagem = mensagem;
        AlphadigiId = alphadigiId;
    }
}
