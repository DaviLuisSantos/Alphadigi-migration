using Alphadigi_migration.Domain.DTOs.Veiculos;
using Alphadigi_migration.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Alphadigi_migration.Domain.Interfaces;

public interface IVeiculoRepository
{
    Task<List<Veiculo>> GetVeiculosAsync();
    Task<List<VeiculoInfoSendAlphadigi>> GetVeiculosSendAsync(int lastId);
    Task<Veiculo> GetByPlateAsync(string plate, int minMatchingCharacters);
    Task<bool> UpdateVagaVeiculoAsync(int id, bool dentro);
    Task<bool> UpdateLastAccessAsync(LastAcessUpdateVeiculoDTO lastAccess);
    Task<Veiculo> GetByIdAsync(int id);
}