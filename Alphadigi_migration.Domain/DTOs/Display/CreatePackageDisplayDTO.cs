using Alphadigi_migration.Domain.Interfaces;

namespace Alphadigi_migration.Domain.DTOs.Display;

public class CreatePackageDisplayDTO 
{
    public string Mensagem { get; set; } 
    public int Linha { get; set; }
    public string Cor { get; set; } 
    public int Tempo { get; set; }
    public int Estilo { get; set; }
}
