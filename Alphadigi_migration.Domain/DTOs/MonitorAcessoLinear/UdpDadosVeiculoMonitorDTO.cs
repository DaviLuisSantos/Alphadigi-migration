using Newtonsoft.Json;
namespace Alphadigi_migration.Domain.DTOs.MonitorAcessoLinear;

public class UdpDadosVeiculoMonitorDTO
{
    [JsonProperty("TTVAGAS")]
    public string TotalVagas { get; set; }
    public string CorAviso { get; set; }
    public bool AvisoVisible { get; set; }
    public string Obs { get; set; }
    public string DadosVeiculo { get; set; }
}
