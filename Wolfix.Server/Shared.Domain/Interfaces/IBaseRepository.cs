using Shared.Domain.Entities;

namespace Shared.Domain.Interfaces;

public interface IBaseRepository<TEntity> where TEntity : BaseEntity
{
    Task AddAsync(TEntity entity,  CancellationToken cancellationToken);
    Task UpdateAsync(TEntity entity, Action updateAction, CancellationToken cancellationToken);
    Task DeleteAsync(TEntity entity,  CancellationToken cancellationToken);
    Task<TEntity?> GetByIdAsync(Guid id, CancellationToken cancellationToken);
    Task<TEntity?> GetByIdAsNoTrackingAsync(Guid id, CancellationToken cancellationToken);
}