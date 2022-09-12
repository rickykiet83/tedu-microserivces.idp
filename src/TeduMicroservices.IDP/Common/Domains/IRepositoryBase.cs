using System.Data;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore.Storage;

namespace TeduMicroservices.IDP.Common.Domains;

public interface IRepositoryBase<T, K>
    where T : EntityBase<K>
{
    #region Query

    IQueryable<T> FindAll(bool trackChanges = false);
    IQueryable<T> FindAll(bool trackChanges = false, params Expression<Func<T, object>>[] includeProperties);
    IQueryable<T> FindByCondition(Expression<Func<T, bool>> expression, bool trackChanges = false);

    IQueryable<T> FindByCondition(Expression<Func<T, bool>> expression, bool trackChanges = false,
        params Expression<Func<T, object>>[] includeProperties);

    Task<T?> GetByIdAsync(K id);
    Task<T?> GetByIdAsync(K id, params Expression<Func<T, object>>[] includeProperties);

    #endregion

    #region Action

    Task<K> CreateAsync(T entity);
    Task UpdateAsync(T entity);
    Task UpdateListAsync(IEnumerable<T> entities);
    Task DeleteAsync(T entity);
    Task DeleteListAsync(IEnumerable<T> entities);

    #endregion
    
    #region Dapper

    Task<IReadOnlyList<T>> QueryAsync<T>(string sql, object param = null, 
        IDbTransaction transaction = null, CancellationToken cancellationToken = default);

    Task<T> QueryFirstOrDefaultAsync<T>(string sql, object param = null, 
        IDbTransaction transaction = null, CancellationToken cancellationToken = default);

    Task<T> QuerySingleAsync<T>(string sql, object param = null, 
        IDbTransaction transaction = null, CancellationToken cancellationToken = default);

    Task<int> ExecuteAsync(string sql, object param = null, 
        IDbTransaction transaction = null, CommandType? commandType = null,
        int? commandTimeout = null);

    #endregion Dapper

    Task<int> SaveChangesAsync();
    Task<IDbContextTransaction> BeginTransactionAsync();
    Task EndTransactionAsync();
    Task RollbackTransactionAsync();
}