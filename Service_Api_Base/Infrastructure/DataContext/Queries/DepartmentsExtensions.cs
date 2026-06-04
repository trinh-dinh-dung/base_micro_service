using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.DataContext.Queries
{
    public static partial class DepartmentsExtensions
    {
        #region Generated Extensions
        public static Domain.Entities.Departments? GetByKey(this System.Linq.IQueryable<Domain.Entities.Departments> queryable, Guid departmentId)
        {
            if (queryable is null)
                throw new ArgumentNullException(nameof(queryable));

            if (queryable is DbSet<Domain.Entities.Departments> dbSet)
                return dbSet.Find(departmentId);

            return queryable.FirstOrDefault(q => q.DepartmentId == departmentId);
        }

        public static async System.Threading.Tasks.ValueTask<Domain.Entities.Departments?> GetByKeyAsync(this System.Linq.IQueryable<Domain.Entities.Departments> queryable, Guid departmentId, System.Threading.CancellationToken cancellationToken = default)
        {
            if (queryable is null)
                throw new ArgumentNullException(nameof(queryable));

            if (queryable is DbSet<Domain.Entities.Departments> dbSet)
                return await dbSet.FindAsync(new object[] { departmentId }, cancellationToken);

            return await queryable.FirstOrDefaultAsync(q => q.DepartmentId == departmentId, cancellationToken);
        }

        #endregion

    }
}
