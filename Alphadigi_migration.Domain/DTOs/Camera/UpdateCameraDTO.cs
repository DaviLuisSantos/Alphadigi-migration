
namespace Alphadigi_migration.Domain.DTOs.Camera;

public class UpdateCameraDTO
{
    public  int Id { get; set; }
    public string Nome { get; set; }
    public string Ip { get; set; }
    public int IdArea { get; set; }
    public string Modelo { get; set; }
    public string Direcao { get; set; }
    public bool FotoEvento { get; set; }

}
