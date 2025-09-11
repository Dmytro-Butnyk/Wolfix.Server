using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Shared.Domain.Entities;
using Shared.Domain.Interfaces;

namespace Shared.Infrastructure.Repositories;

//TODO: REMOVE SAVE CHANGES

public class BaseRepository<TContext, TEntity>(TContext context)
    : IBaseRepository<TEntity>
    where TEntity : BaseEntity
    where TContext : DbContext
{
    private readonly DbSet<TEntity> _dbSet = context.Set<TEntity>();
    public TContext Context => context;

    public async Task<bool> IsExistAsync(Guid id, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        
        return await _dbSet
            .AsNoTracking()
            .AnyAsync(e => e.Id == id, cancellationToken);
    }

    public async Task AddAsync(TEntity entity, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        
        _dbSet.Add(entity);
        await context.SaveChangesAsync(cancellationToken);
    } 
    
    public async Task UpdateAsync(TEntity entity, Action actionUpdate, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        
        _dbSet.Update(entity);
        actionUpdate.Invoke();
        await context.SaveChangesAsync(cancellationToken);
    } 
    
    public async Task DeleteAsync(TEntity entity, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        
        _dbSet.Remove(entity);
        await context.SaveChangesAsync(cancellationToken);
    }

    public async Task<TEntity?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        
        TEntity? entity = await _dbSet.FirstOrDefaultAsync(e => e.Id == id, cancellationToken);
        return entity;
    }

    public async Task<TEntity?> GetByIdAsNoTrackingAsync(Guid id, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        
        TEntity? entity = await _dbSet
            .AsNoTracking()
            .FirstOrDefaultAsync(e => e.Id == id, cancellationToken);
        
        return entity;
    }

    public async Task ExecuteDeleteAsync(CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        await _dbSet.ExecuteDeleteAsync(cancellationToken);
    }
    
    public async Task ExecuteDeleteAsync(Expression<Func<TEntity,bool>> condition, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        await _dbSet
            .Where(condition)
            .ExecuteDeleteAsync(cancellationToken);
    }

    public async Task SaveChangesAsync(CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        
        await context.SaveChangesAsync(cancellationToken);
    }
}