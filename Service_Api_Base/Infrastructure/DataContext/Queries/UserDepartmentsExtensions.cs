using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.DataContext.Queries
{
    public static partial class UserDepartmentsExtensions
    {
        #region Generated Extensions
        public static IQueryable<Domain.Entities.UserDepartments> ByDepartmentId(this IQueryable<Domain.Entities.UserDepartments> queryable, Guid departmentId)
        {
            return queryable.Where(q => q.DepartmentId == departmentId);
        }

        public static IQueryable<Domain.Entities.UserDepartments> ByPositionId(this IQueryable<Domain.Entities.UserDepartments> queryable, Guid? positionId)
        {
            return queryable.Where(q => (q.PositionId == positionId || (positionId == null && q.PositionId == null)));
        }

        public static IQueryable<Domain.Entities.UserDepartments> ByUserId(this IQueryable<Domain.Entities.UserDepartments> queryable, Guid userId)
        {
            return queryable.Where(q => q.UserId == userId);
        }

        public static Domain.Entities.UserDepartments GetByKey(this IQueryable<Domain.Entities.UserDepartments> queryable, Guid userId, Guid departmentId)
        {
            if (queryable is DbSet<Domain.Entities.UserDepartments> dbSet)
                return dbSet.Find(userId, departmentId);

            return queryable.FirstOrDefault(q => q.UserId == userId
                && q.DepartmentId == departmentId);
        }

        public static ValueTask<Domain.Entities.UserDepartments> GetByKeyAsync(this IQueryable<Domain.Entities.UserDepartments> queryable, Guid userId, Guid departmentId)
        {
            if (queryable is DbSet<Domain.Entities.UserDepartments> dbSet)
                return dbSet.FindAsync(userId, departmentId);

            var task = queryable.FirstOrDefaultAsync(q => q.UserId == userId
                && q.DepartmentId == departmentId);
            return new ValueTask<Domain.Entities.UserDepartments>(task);
        }

        #endregion

    }
}
