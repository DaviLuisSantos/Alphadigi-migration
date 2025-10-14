using Alphadigi_migration.Domain.EntitiesNew;

namespace Alphadigi_migration.Domain.Interfaces
{
    public interface IVisitaSaiuSemControleRepository
    {
        Task<bool> SalvarRegistroSaidaAsync(VisitaSaiuSemControle registro);
    }
}