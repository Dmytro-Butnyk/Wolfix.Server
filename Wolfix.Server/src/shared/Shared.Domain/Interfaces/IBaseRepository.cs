using System.Linq.Expressions;
using Shared.Domain.Entities;

namespace Shared.Domain.Interfaces;

public interface IBaseRepository<TEntity> where TEntity : BaseEntity
{
    Task<bool> IsExistAsync(Guid id, CancellationToken cancellationToken);
    
    Task AddAsync(TEntity entity,  CancellationToken cancellationToken);
    void Update(TEntity entity, Action updateAction, CancellationToken cancellationToken);
    void Delete(TEntity entity, CancellationToken cancellationToken);
    
    Task<TEntity?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken,
        params string[]? includeProperties);
    Task<TEntity?> GetByIdAsNoTrackingAsync(Guid id,
        CancellationToken cancellationToken,
        params string[]? includeProperties);
    
    Task ExecuteDeleteAsync(CancellationToken cancellationToken);
    Task ExecuteDeleteAsync(Expression<Func<TEntity,bool>> condition, CancellationToken cancellationToken);
    
    Task SaveChangesAsync(CancellationToken cancellationToken);
}