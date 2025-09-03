using Alphadigi_migration.Domain.EntitiesNew;

namespace Alphadigi_migration.Domain.DTOs.MonitorAcessoLinear;

public class DadosVeiculoMonitorDTO
{

    public Veiculo Veiculo { get; set; }
    public string Ip { get; set; }
    public string Acesso { get; set; }
    public DateTime HoraAcesso { get; set; }

}
