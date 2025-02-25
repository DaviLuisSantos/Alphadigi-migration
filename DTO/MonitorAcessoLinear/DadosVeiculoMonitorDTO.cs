using Alphadigi_migration.Models;

namespace Alphadigi_migration.DTO.MonitorAcessoLinear;

public class DadosVeiculoMonitorDTO
{

    public Models.Veiculo Veiculo { get; set; }
    public string Ip { get; set; }
    public string Acesso { get; set; }
    public DateTime HoraAcesso { get; set; }

}
