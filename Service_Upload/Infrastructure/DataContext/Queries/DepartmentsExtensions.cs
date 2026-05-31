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
        public static Domain.Entities.Departments GetByKey(this IQueryable<Domain.Entities.Departments> queryable, Guid departmentId)
        {
            if (queryable is DbSet<Domain.Entities.Departments> dbSet)
                return dbSet.Find(departmentId);

            return queryable.FirstOrDefault(q => q.DepartmentId == departmentId);
        }

        public static ValueTask<Domain.Entities.Departments> GetByKeyAsync(this IQueryable<Domain.Entities.Departments> queryable, Guid departmentId)
        {
            if (queryable is DbSet<Domain.Entities.Departments> dbSet)
                return dbSet.FindAsync(departmentId);

            var task = queryable.FirstOrDefaultAsync(q => q.DepartmentId == departmentId);
            return new ValueTask<Domain.Entities.Departments>(task);
        }

        #endregion

    }
}
