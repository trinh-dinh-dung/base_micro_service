using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.EntityFrameworkCore;

namespace Infrastructure.DataContext.Queries
{
    public static partial class EntityAuditLogsExtensions
    {
        #region Generated Extensions
        public static System.Linq.IQueryable<Domain.Entities.EntityAuditLogs> ByChangedAtUtc(this System.Linq.IQueryable<Domain.Entities.EntityAuditLogs> queryable, DateTimeOffset changedAtUtc)
        {
            if (queryable is null)
                throw new ArgumentNullException(nameof(queryable));

            return queryable.Where(q => q.ChangedAtUtc == changedAtUtc);
        }

        public static System.Linq.IQueryable<Domain.Entities.EntityAuditLogs> ByEntityName(this System.Linq.IQueryable<Domain.Entities.EntityAuditLogs> queryable, string entityName)
        {
            if (queryable is null)
                throw new ArgumentNullException(nameof(queryable));

            return queryable.Where(q => q.EntityName == entityName);
        }

        public static Domain.Entities.EntityAuditLogs? GetByKey(this System.Linq.IQueryable<Domain.Entities.EntityAuditLogs> queryable, Guid id)
        {
            if (queryable is null)
                throw new ArgumentNullException(nameof(queryable));

            if (queryable is DbSet<Domain.Entities.EntityAuditLogs> dbSet)
                return dbSet.Find(id);

            return queryable.FirstOrDefault(q => q.Id == id);
        }

        public static async System.Threading.Tasks.ValueTask<Domain.Entities.EntityAuditLogs?> GetByKeyAsync(this System.Linq.IQueryable<Domain.Entities.EntityAuditLogs> queryable, Guid id, System.Threading.CancellationToken cancellationToken = default)
        {
            if (queryable is null)
                throw new ArgumentNullException(nameof(queryable));

            if (queryable is DbSet<Domain.Entities.EntityAuditLogs> dbSet)
                return await dbSet.FindAsync(new object[] { id }, cancellationToken);

            return await queryable.FirstOrDefaultAsync(q => q.Id == id, cancellationToken);
        }

        public static System.Linq.IQueryable<Domain.Entities.EntityAuditLogs> ByPageCode(this System.Linq.IQueryable<Domain.Entities.EntityAuditLogs> queryable, string pageCode)
        {
            if (queryable is null)
                throw new ArgumentNullException(nameof(queryable));

            return queryable.Where(q => (q.PageCode == pageCode || (pageCode == null && q.PageCode == null)));
        }

        #endregion

    }
}
