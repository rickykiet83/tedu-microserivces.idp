using System.Data;
using System.Linq.Expressions;
using Dapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using TeduMicroservices.IDP.Infrastructure.Exceptions;
using TeduMicroservices.IDP.Persistence;

namespace TeduMicroservices.IDP.Infrastructure.Domains;

public class RepositoryBase<T, K> : IRepositoryBase<T, K>
    where T : EntityBase<K>
{
    private readonly TeduIdentityContext _dbContext;
    private readonly IUnitOfWork _unitOfWork;

    public RepositoryBase(TeduIdentityContext dbContext,
        IUnitOfWork unitOfWork)
    {
        _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
    }

    #region Query

    public IQueryable<T> FindAll(bool trackChanges = false) =>
        !trackChanges ? _dbContext.Set<T>().AsNoTracking() : 
            _dbContext.Set<T>();

    public IQueryable<T> FindAll(bool trackChanges = false, 
        params Expression<Func<T, object>>[] includeProperties)
    {
        var items = FindAll(trackChanges);
        items = includeProperties.Aggregate(items, (current, includeProperty) => current.Include(includeProperty));
        return items;
    }

    public IQueryable<T> FindByCondition(Expression<Func<T, bool>> expression, 
        bool trackChanges = false)
    => !trackChanges
        ? _dbContext.Set<T>().Where(expression).AsNoTracking()
        : _dbContext.Set<T>().Where(expression);

    public IQueryable<T> FindByCondition(Expression<Func<T, bool>> expression, 
        bool trackChanges = false,
        params Expression<Func<T, object>>[] includeProperties)
    {
        var items = FindByCondition(expression, trackChanges);
        items = includeProperties.Aggregate(items, (current, includeProperty) => current.Include(includeProperty));
        return items;
    }

    public Task<T> GetByIdAsync(K id) 
        => FindByCondition(x => x.Id.Equals(id)).FirstOrDefaultAsync();

    public Task<T> GetByIdAsync(K id, params Expression<Func<T, object>>[] includeProperties)
        => FindByCondition(x => x.Id.Equals(id), trackChanges:false, includeProperties)
            .FirstOrDefaultAsync();

    #endregion

    #region Action

    public async Task<K> CreateAsync(T entity)
    {
        await _dbContext.Set<T>().AddAsync(entity);
        await SaveChangesAsync();
        return entity.Id;
    }

    public async Task UpdateAsync(T entity)
    {
        if (_dbContext.Entry(entity).State == EntityState.Unchanged) return;
        
        T exist = await _dbContext.Set<T>().FindAsync(entity.Id);
        _dbContext.Entry(exist).CurrentValues.SetValues(entity);
        await SaveChangesAsync();
    }

    public async Task UpdateListAsync(IEnumerable<T> entities)
    {
        await _dbContext.Set<T>().AddRangeAsync(entities);
        await SaveChangesAsync();
    }

    public async Task DeleteAsync(T entity)
    {
        _dbContext.Set<T>().Remove(entity);
        await SaveChangesAsync();
    }

    public async Task DeleteListAsync(IEnumerable<T> entities)
    {
        _dbContext.Set<T>().RemoveRange(entities);
        await SaveChangesAsync();
    }

    #endregion
    
    #region Dapper

    public async Task<IReadOnlyList<T>> QueryAsync<T>(string sql, object param = null, IDbTransaction transaction = null,
        CancellationToken cancellationToken = default)
    {
        return (await _dbContext.Connection.QueryAsync<T>(sql, param, transaction))
            .AsList();
    }

    public async Task<T> QueryFirstOrDefaultAsync<T>(string sql, object param = null, IDbTransaction transaction = null,
        CancellationToken cancellationToken = default)
    {
        var entity = await _dbContext.Connection.QueryFirstOrDefaultAsync<T>(sql, param, transaction);
        if (entity == null) throw new EntityNotFoundException();
        return entity;
    }

    public async Task<T> QuerySingleAsync<T>(string sql, object param = null, IDbTransaction transaction = null,
        CancellationToken cancellationToken = default)
    {
        return await _dbContext.Connection.QuerySingleAsync<T>(sql, param, transaction);
    }

    public async Task<int> ExecuteAsync(string sql, object param = null, IDbTransaction transaction = null, CommandType? commandType = CommandType.StoredProcedure, int? commandTimeout = null)
    {
        return await _dbContext.Connection.ExecuteAsync(sql, param, transaction, commandTimeout, commandType);
    }

    #endregion Dapper

    public Task<int> SaveChangesAsync() => _unitOfWork.CommitAsync();

    public Task<IDbContextTransaction> BeginTransactionAsync() 
        => _dbContext.Database.BeginTransactionAsync();

    public async Task EndTransactionAsync()
    { 
        await SaveChangesAsync();
        await _dbContext.Database.CommitTransactionAsync();
    }

    public Task RollbackTransactionAsync() 
        => _dbContext.Database.RollbackTransactionAsync();
}