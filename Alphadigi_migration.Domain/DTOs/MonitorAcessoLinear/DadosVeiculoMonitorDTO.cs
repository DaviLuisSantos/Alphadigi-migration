using Alphadigi_migration.Domain.Entities;
using Alphadigi_migration.Models;

namespace Alphadigi_migration.Domain.DTOs.MonitorAcessoLinear;

public class DadosVeiculoMonitorDTO
{

    public Alphadigi_migration.Domain.Entities.Veiculo Veiculo { get; set; }
    public string Ip { get; set; }
    public string Acesso { get; set; }
    public DateTime HoraAcesso { get; set; }

}
