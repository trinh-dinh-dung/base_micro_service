using Microsoft.EntityFrameworkCore.Query;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Abstractions.Persistence
{
    public interface IGenericRepository<TEntity> where TEntity : class
    {
        Task<int> Count(Expression<Func<TEntity, bool>> filter = null, CancellationToken cancellationToken = default);
        IQueryable<TEntity> Get(Expression<Func<TEntity, bool>> filter = null,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
            Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>> include = null,
            bool enableTracking = true, int offset = 0, int limit = -1, bool asSplitQuery = false);

        IQueryable<TEntity> Get(out int count, Expression<Func<TEntity, bool>> filter = null,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
            Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>> include = null,
            bool enableTracking = true, int offset = 0, int limit = -1, bool asSplitQuery = false);

        Task<TEntity> FirstOrDefault(Expression<Func<TEntity, bool>> filter = null,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
            Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>> include = null,
            bool enableTracking = true, bool asSplitQuery = false, CancellationToken cancellationToken = default);

        Task Insert(TEntity entity, CancellationToken cancellationToken = default);
        Task InsertRange(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default);
        Task Delete(object id, CancellationToken cancellationToken = default);
        void Delete(TEntity entityToDelete);
        void DeleteRange(IEnumerable<TEntity> entities);
        void Update(TEntity entityToUpdate);
        void UpdateRange(IEnumerable<TEntity> entityToUpdates);
        IQueryable<TEntity> GetDataFromSqlRaw(string sqlQuery, object[] parameters = null);

        Task<List<T>> RawSqlQuery<T>(string query, Func<DbDataReader, T> map, object[] parameter = null, CancellationToken cancellationToken = default);
        Task<int> ExecuteSqlRawAsync(string sqlQuery, object[] parameters = null, CancellationToken cancellationToken = default);
    }
}
