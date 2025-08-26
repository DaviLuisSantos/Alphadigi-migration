using Alphadigi_migration.Domain.Entities;
using Alphadigi_migration.Models;

 namespace Alphadigi_migration.Domain.Interfaces;



public interface IAccessHandler
{
    Task<(bool ShouldReturn, string Acesso)> HandleAccessAsync(Veiculo veiculo, Alphadigi_migration.Domain.Entities.Alphadigi alphadigi);
}
