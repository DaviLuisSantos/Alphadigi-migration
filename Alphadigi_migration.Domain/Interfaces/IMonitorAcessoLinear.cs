using Alphadigi_migration.Domain.DTOs.Veiculos;
using Alphadigi_migration.Domain.EntitiesNew;


namespace Alphadigi_migration.Domain.Interfaces;

public interface IMonitorAcessoLinear
{
    Task<bool> DadosVeiculo(DadosVeiculoMonitorDTO monitorAcesso);

   // Task<Unidade> GetUnidadeByNome(string unidade);

    //Task EnviarDadosVeiculoParaMonitor(string placa, string acesso, string ipCamera);
}
