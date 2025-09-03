using Alphadigi_migration.Domain.EntitiesNew;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Alphadigi_migration.Domain.DTOs.Veiculos;

public class DadosVeiculoMonitorDTO
{
    public Veiculo Veiculo { get; set; }
    public string Ip { get; set; } = string.Empty;
    public string Acesso { get; set; } = string.Empty;
    public DateTime HoraAcesso { get; set; }
}
