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

    public string Marca { get; set; }
    public string Cor { get; set; }

    public bool IsVisitante { get; set; } = false;
    public string VisitanteNome { get; set; }
    public string VisitanteUnidade { get; set; }


}
