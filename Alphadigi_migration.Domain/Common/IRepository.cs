// Domain/Common/IRepository.cs
namespace Alphadigi.Domain.Common;

public interface IRepository<T> where T : EntityBase
{
    Task<T?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IEnumerable<T>> GetAllAsync(CancellationToken cancellationToken = default);
    Task AddAsync(T entity, CancellationToken cancellationToken = default);
    void Update(T entity);
    void Delete(T entity);
    Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken = default);
}