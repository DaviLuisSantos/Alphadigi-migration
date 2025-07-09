using Alphadigi_migration.Models;

namespace Alphadigi_migration.Interfaces;

public interface IAccessHandler
{
    Task<(bool ShouldReturn, string Acesso)> HandleAccessAsync(Veiculo veiculo, Alphadigi alphadigi);
}
