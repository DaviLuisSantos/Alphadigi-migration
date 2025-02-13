using Alphadigi_migration.Models;
using Microsoft.EntityFrameworkCore;

namespace Alphadigi_migration.Services;

public interface IVeiculoService
{
    Task<List<Veiculo>> GetVeiculos();
    Task<List<VeiculoInfo>> GetVeiculosSend(int lastId);
    Task<Veiculo> getByPlate(string plate);

    public class VeiculoInfo
    {
        public int Id { get; set; }
        public string Placa { get; set; }
    }
}