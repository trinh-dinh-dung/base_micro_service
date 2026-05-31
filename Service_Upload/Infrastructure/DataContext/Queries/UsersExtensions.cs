using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.DataContext.Queries
{
    public static partial class UsersExtensions
    {
        #region Generated Extensions
        public static Domain.Entities.Users GetByKey(this IQueryable<Domain.Entities.Users> queryable, Guid userId)
        {
            if (queryable is DbSet<Domain.Entities.Users> dbSet)
                return dbSet.Find(userId);

            return queryable.FirstOrDefault(q => q.UserId == userId);
        }

        public static ValueTask<Domain.Entities.Users> GetByKeyAsync(this IQueryable<Domain.Entities.Users> queryable, Guid userId)
        {
            if (queryable is DbSet<Domain.Entities.Users> dbSet)
                return dbSet.FindAsync(userId);

            var task = queryable.FirstOrDefaultAsync(q => q.UserId == userId);
            return new ValueTask<Domain.Entities.Users>(task);
        }

        #endregion

    }
}
