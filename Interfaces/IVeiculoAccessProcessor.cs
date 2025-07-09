using Alphadigi_migration.Models;

namespace Alphadigi_migration.Interfaces;

public interface IVeiculoAccessProcessor
{
    Task<(bool ShouldReturn, string Acesso)> ProcessVeiculoAccessAsync(Veiculo veiculo, Alphadigi alphadigi, DateTime timestamp);
}
