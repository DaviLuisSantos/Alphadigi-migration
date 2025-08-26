using System.ComponentModel.DataAnnotations.Schema;


namespace Alphadigi_migration.Application.DTOs.PlacaLida;

public class CreateMoradorAcessoDTO
{
    public string Unidade { get; set; }
    public DateTime DataHora { get; set; } = DateTime.Now;
    public string Operador { get; set; }
    public string Local { get; set; }
    public string Nome { get; set; }
    public bool Abriu { get; set; }
    public string Foto { get; set; }

    public CreateMoradorAcessoDTO(string unidade, string operador, string local, string nome, bool abriu, string foto)
    {
        Unidade = unidade;
        Operador = operador;
        Local = local;
        Nome = nome;
        Foto = foto;
        Abriu = abriu;
    }
}
