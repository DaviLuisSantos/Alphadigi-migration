using Alphadigi_migration.Domain.Entities;
using Alphadigi_migration.Models;

namespace Alphadigi_migration.Domain.Interfaces;

public interface IVeiculoAccessProcessor
{
    Task<(bool ShouldReturn, string Acesso)> ProcessVeiculoAccessAsync(Veiculo veiculo, Alphadigi_migration.Domain.Entities.Alphadigi alphadigi, DateTime timestamp);
}
