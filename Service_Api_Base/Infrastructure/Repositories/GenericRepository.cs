using Application.Abstractions.Persistence;
using Infrastructure.DataContext;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace Infrastructure.Repositories
{
    public class GenericRepository<TEntity> : IGenericRepository<TEntity> where TEntity : class
    {
        internal readonly PVIContext _context;
        internal readonly DbSet<TEntity> _dbSet;

        public GenericRepository(PVIContext context)
        {
            _context = context;
            _dbSet = context.Set<TEntity>();
        }

        public virtual async Task<int> Count(Expression<Func<TEntity, bool>> filter = null, CancellationToken cancellationToken = default)
        {
            IQueryable<TEntity> query = _dbSet.AsNoTracking();
            if (filter != null)
                query = query.Where(filter);
            return await query.CountAsync(cancellationToken);
        }

        public virtual IQueryable<TEntity> Get(Expression<Func<TEntity, bool>> filter = null,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
            Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>> include = null,
            bool enableTracking = true, int offset = 0, int limit = -1, bool asSplitQuery = false)
        {
            IQueryable<TEntity> query = _dbSet;
            if (!enableTracking) query = query.AsNoTracking();

            if (filter != null)
                query = query.Where(filter);

            if (include != null) query = include(query);

            if (orderBy != null)
                query = orderBy(query);

            if (limit != -1)
                query = query.Skip(offset).Take(limit);
            if (asSplitQuery)
                query = query.AsSplitQuery();

            return query;
        }

        public virtual IQueryable<TEntity> Get(out int count, Expression<Func<TEntity, bool>> filter = null,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
            Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>> include = null,
            bool enableTracking = true, int offset = 0, int limit = -1, bool asSplitQuery = false)
        {
            IQueryable<TEntity> query = _dbSet;
            if (!enableTracking) query = query.AsNoTracking();
            if (filter != null)
                query = query.Where(filter);

            if (include != null) query = include(query);

            count = query.Count();

            if (orderBy != null)
                query = orderBy(query);

            if (limit != -1)
                query = query.Skip(offset).Take(limit);
            if (asSplitQuery)
                query = query.AsSplitQuery();

            return query;
        }

        public virtual async Task<TEntity> FirstOrDefault(Expression<Func<TEntity, bool>> filter = null,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
            Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>> include = null,
            bool enableTracking = true, bool asSplitQuery = false, CancellationToken cancellationToken = default)
        {
            IQueryable<TEntity> query = _dbSet;
            if (!enableTracking) query = query.AsNoTracking();

            if (filter != null)
                query = query.Where(filter);

            if (include != null) query = include(query);

            if (orderBy != null)
                query = orderBy(query);

            if (asSplitQuery)
                query = query.AsSplitQuery();
            return await query.FirstOrDefaultAsync(cancellationToken);
        }

        public virtual async Task Insert(TEntity entity, CancellationToken cancellationToken = default) => await _dbSet.AddAsync(entity, cancellationToken);

        public virtual async Task InsertRange(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default) => await _dbSet.AddRangeAsync(entities, cancellationToken);

        public virtual async Task Delete(object id, CancellationToken cancellationToken = default)
        {
            var entityToDelete = await _dbSet.FindAsync(new[] { id }, cancellationToken);
            Delete(entityToDelete);
        }

        public virtual void Delete(TEntity entityToDelete)
        {
            if (_context.Entry(entityToDelete).State == EntityState.Detached)
                _dbSet.Attach(entityToDelete);
            _dbSet.Remove(entityToDelete);
        }

        public virtual void DeleteRange(IEnumerable<TEntity> entities) => _dbSet.RemoveRange(entities);

        public virtual void Update(TEntity entityToUpdate)
        {
            _dbSet.Attach(entityToUpdate);

            var rowVersionProperty = typeof(TEntity).GetProperty("RowVersion");
            if (rowVersionProperty != null)
            {
                var rowVersionValue = rowVersionProperty.GetValue(entityToUpdate);
                _context.Entry(entityToUpdate).OriginalValues["RowVersion"] = rowVersionValue;
            }

            _context.Entry(entityToUpdate).State = EntityState.Modified;
        }

        public virtual void UpdateRange(IEnumerable<TEntity> entityToUpdates)
        {
            if (entityToUpdates == null) return;

            _dbSet.AttachRange(entityToUpdates);
            foreach (var entity in entityToUpdates)
            {
                var rowVersionProperty = typeof(TEntity).GetProperty("RowVersion");
                if (rowVersionProperty != null)
                {
                    var rowVersionValue = rowVersionProperty.GetValue(entity);
                    _context.Entry(entity).OriginalValues["RowVersion"] = rowVersionValue;
                }
                _context.Entry(entity).State = EntityState.Modified;
            }
        }

        public virtual IQueryable<TEntity> GetDataFromSqlRaw(string sqlQuery, object[] parameters = null)
        {
            if (parameters == null)
                return _dbSet.FromSqlRaw(sqlQuery);
            return _dbSet.FromSqlRaw(sqlQuery, parameters);
        }

        public async Task<List<T>> RawSqlQuery<T>(string query, Func<DbDataReader, T> map, object[] parameter = null, CancellationToken cancellationToken = default)
        {
            await using var command = _context.Database.GetDbConnection().CreateCommand();
            if (parameter != null)
                command.Parameters.AddRange(parameter);
            command.CommandText = query;
            command.CommandType = CommandType.Text;
            await _context.Database.OpenConnectionAsync(cancellationToken);
            await using var result = await command.ExecuteReaderAsync(cancellationToken);
            var entities = new List<T>();
            while (await result.ReadAsync(cancellationToken))
                entities.Add(map(result));
            return entities;
        }

        public virtual async Task<int> ExecuteSqlRawAsync(string sqlQuery, object[] parameters = null, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrEmpty(sqlQuery))
                return 0;

            if (parameters == null)
                return await _context.Database.ExecuteSqlRawAsync(sqlQuery, cancellationToken);
            return await _context.Database.ExecuteSqlRawAsync(sqlQuery, parameters, cancellationToken);
        }
    }
}
