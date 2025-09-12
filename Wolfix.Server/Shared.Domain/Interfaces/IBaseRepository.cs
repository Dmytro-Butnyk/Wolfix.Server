using System.Linq.Expressions;
using Shared.Domain.Entities;

namespace Shared.Domain.Interfaces;

public interface IBaseRepository<TEntity> where TEntity : BaseEntity
{
    Task<bool> IsExistAsync(Guid id, CancellationToken cancellationToken);
    
    Task AddAsync(TEntity entity,  CancellationToken cancellationToken);
    Task UpdateAsync(TEntity entity, Action updateAction, CancellationToken cancellationToken);
    Task DeleteAsync(TEntity entity,  CancellationToken cancellationToken);
    //TODO: add include parameter
    Task<TEntity?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken,
        Func<IQueryable<TEntity>, IQueryable<TEntity>>? include = null);
    Task<TEntity?> GetByIdAsNoTrackingAsync(Guid id,
        CancellationToken cancellationToken,
        Func<IQueryable<TEntity>, IQueryable<TEntity>>? include = null);
    
    Task ExecuteDeleteAsync(CancellationToken cancellationToken);
    Task ExecuteDeleteAsync(Expression<Func<TEntity,bool>> condition, CancellationToken cancellationToken);
    
    Task SaveChangesAsync(CancellationToken cancellationToken);
}