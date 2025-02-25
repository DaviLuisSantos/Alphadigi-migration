using Alphadigi_migration.DTO.Veiculo;
using Alphadigi_migration.Models;

namespace Alphadigi_migration.Services;

public interface IVeiculoService
{
    Task<List<Veiculo>> GetVeiculos();
    Task<List<VeiculoInfo>> GetVeiculosSend(int lastId);
    Task<Veiculo> getByPlate(string plate);
    Task<bool> UpdateVagaVeiculo(int id, bool dentro);
    Task<bool> UpdateLastAccess(LastAcessUpdateVeiculoDTO lastAccess);
    string prepareVeiculoDataString(Veiculo veiculo);

    public class VeiculoInfo
    {
        public int Id { get; set; }
        public string Placa { get; set; }
    }
}