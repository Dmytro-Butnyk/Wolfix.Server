using Microsoft.EntityFrameworkCore;
using Wolfix.Domain.Shared;
using Wolfix.Domain.Shared.Interfaces;

namespace Wolfix.Infrastructure.Shared.Database;

public abstract class BaseRepository<TEntity>(WolfixStoreContext context)
    : IBaseRepository<TEntity> where TEntity: BaseEntity
{
    //todo: если будет проблема с репозиториями, то она тут
    
    private readonly DbSet<TEntity> _dbSet = context.Set<TEntity>();
    public async Task AddAsync(TEntity entity, CancellationToken cancellationToken)
    {
        _dbSet.Add(entity);
        await context.SaveChangesAsync(cancellationToken);
    } 
    public async Task UpdateAsync(TEntity entity, Action actionUpdate, CancellationToken cancellationToken)
    {
        _dbSet.Update(entity);
        actionUpdate.Invoke();
        await context.SaveChangesAsync(cancellationToken);
    } 
    public async Task DeleteAsync(TEntity entity, CancellationToken cancellationToken)
    {
        _dbSet.Remove(entity);
        await context.SaveChangesAsync(cancellationToken);
    }

    public async Task<TEntity?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        TEntity? entity = await _dbSet.FirstOrDefaultAsync(e => e.Id == id, cancellationToken);
        return entity;
    }

    public async Task<TEntity?> GetByIdAsNoTrackingAsync(Guid id, CancellationToken cancellationToken)
    {
        TEntity? entity = await _dbSet.AsNoTracking()
            .FirstOrDefaultAsync(e => e.Id == id, cancellationToken);
        return entity;
    }
}