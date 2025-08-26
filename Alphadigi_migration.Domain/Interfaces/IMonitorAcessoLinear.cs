using Alphadigi_migration.Domain.DTOs.Veiculos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Alphadigi_migration.Domain.Interfaces;

public interface IMonitorAcessoLinear
{
    Task<bool> DadosVeiculo(DadosVeiculoMonitorDTO monitorAcesso);
}
