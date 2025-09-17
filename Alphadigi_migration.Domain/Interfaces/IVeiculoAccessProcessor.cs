using Alphadigi_migration.Domain.EntitiesNew;


namespace Alphadigi_migration.Domain.Interfaces;

public interface IVeiculoAccessProcessor
{
    Task<(bool ShouldReturn, string Acesso)> ProcessVeiculoAccessAsync(Veiculo veiculo, 
                                                                       Alphadigi_migration.Domain.EntitiesNew.Alphadigi alphadigi, 
                                                                       DateTime timestamp);
}
