using Alphadigi_migration.Domain.DTOs.Veiculos;

using Veiculo = Alphadigi_migration.Domain.EntitiesNew.Veiculo;

namespace Alphadigi_migration.Domain.Interfaces;

public interface IVeiculoRepository
{
    Task<List<Veiculo>> GetVeiculosAsync();
    Task<List<VeiculoInfoSendAlphadigi>> GetVeiculosSendAsync(int lastId);
    Task<Veiculo> GetByPlateAsync(string plate);
    Task<bool> UpdateVagaVeiculoAsync(int  id, bool dentro);
    Task<bool> UpdateLastAccessAsync(LastAcessUpdateVeiculoDTO lastAccess);
    Task<Veiculo> GetByIdAsync(int id);
}