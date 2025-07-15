using Wolfix.Domain.Shared;
using Wolfix.Domain.Shared.Interfaces;

namespace Wolfix.Infrastructure.Shared.Database;

public sealed class BaseRepository<TEntity> : IBaseRepository<TEntity> where TEntity: BaseEntity
{
    public async Task AddAsync(TEntity entity, CancellationToken cancellationToken)
    {
        
    } 
    public async Task UpdateAsync(TEntity entity, Action actionUpdate, CancellationToken cancellationToken)
    {
        
    } 
    public async Task DeleteAsync(Guid entityId, CancellationToken cancellationToken)
    {
        
    }

    public async Task<TEntity> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
    }

    public async Task<TEntity> GetByIdAsNoTrackingAsync(Guid id, CancellationToken cancellationToken)
    {
    }
}