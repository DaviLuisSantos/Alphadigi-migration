using Alphadigi_migration.Domain.EntitiesNew;


namespace Alphadigi_migration.Domain.DTOs.Veiculos;

public class DadosVeiculoMonitorDTO
{
    public string Placa { get; set; }
    public string Unidade { get; set; }
    public string Acesso { get; set; }
    public DateTime HoraAcesso { get; set; }
    public string Ip { get; set; }
    public string Modelo { get; set; }
    public string Cor { get; set; }


}
