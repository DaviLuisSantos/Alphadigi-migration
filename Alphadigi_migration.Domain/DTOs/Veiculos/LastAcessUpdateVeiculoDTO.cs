using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Alphadigi_migration.Domain.DTOs.Veiculos;

public class LastAcessUpdateVeiculoDTO
{
    public int IdVeiculo { get; set; }
    public string IpCamera { get; set; }
    public DateTime TimeAccess { get; set; }
}
