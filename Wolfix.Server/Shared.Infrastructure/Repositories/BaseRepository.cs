using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Shared.Domain.Entities;
using Shared.Domain.Interfaces;

namespace Shared.Infrastructure.Repositories;

public class BaseRepository<TContext, TEntity>(TContext context)
    : IBaseRepository<TEntity>
    where TEntity : BaseEntity
    where TContext : DbContext
{
    private readonly DbSet<TEntity> _dbSet = context.Set<TEntity>();

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
        
        await _dbSet.AddAsync(entity, cancellationToken);
    } 
    
    public void Update(TEntity entity, Action actionUpdate, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        
        _dbSet.Update(entity);
        actionUpdate.Invoke();
    } 
    
    public void Delete(TEntity entity, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        
        _dbSet.Remove(entity);
    }

    public async Task<TEntity?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken,
        params string[]? includeProperties)
    {
        cancellationToken.ThrowIfCancellationRequested();

        IQueryable<TEntity> query = _dbSet;

        if (includeProperties is not null)
        {
            foreach (string includeProperty in includeProperties)
            {
                query = query.Include(includeProperty);
            }
        }

        return await query.FirstOrDefaultAsync(e => e.Id == id, cancellationToken);
    }

    public async Task<TEntity?> GetByIdAsNoTrackingAsync(
        Guid id,
        CancellationToken cancellationToken,
        params string[]? includeProperties)
    {
        cancellationToken.ThrowIfCancellationRequested();

        IQueryable<TEntity> query = _dbSet.AsNoTracking();

        if (includeProperties is not null)
        {
            foreach (string includeProperty in includeProperties)
            {
                query = query.Include(includeProperty);
            }
        }

        return await query
            .AsNoTracking()
            .FirstOrDefaultAsync(e => e.Id == id, cancellationToken);
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