using Alphadigi_migration.Domain.EntitiesNew;

namespace Alphadigi_migration.Domain.Interfaces;

public interface IAccessHandler
{
    Task<(bool ShouldReturn, string Acesso)> HandleAccessAsync(Veiculo veiculo, 
                                                               Alphadigi_migration.Domain.EntitiesNew.Alphadigi alphadigi);
}
