

namespace Alphadigi_migration.Domain.DTOs.Camera;

public class CreateCameraDTO
{
    public string Nome { get; set; }
    public string Ip { get; set; }
    public int IdArea { get; set; }
    public string Modelo { get; set; }
    public string Direcao { get; set; }
    public bool FotoEvento { get; set; }

    public CreateCameraDTO(string nome, string ip, int idArea, string modelo, string direcao, bool fotoEvento)
    {
        Nome = nome;
        Ip = ip;
        IdArea = idArea;
        Modelo = modelo;
        Direcao = direcao;
        FotoEvento = fotoEvento;
    }
}
