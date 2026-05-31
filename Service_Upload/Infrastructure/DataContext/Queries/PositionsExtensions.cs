using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.DataContext.Queries
{
    public static partial class PositionsExtensions
    {
        #region Generated Extensions
        public static Domain.Entities.Positions GetByKey(this IQueryable<Domain.Entities.Positions> queryable, Guid positionId)
        {
            if (queryable is DbSet<Domain.Entities.Positions> dbSet)
                return dbSet.Find(positionId);

            return queryable.FirstOrDefault(q => q.PositionId == positionId);
        }

        public static ValueTask<Domain.Entities.Positions> GetByKeyAsync(this IQueryable<Domain.Entities.Positions> queryable, Guid positionId)
        {
            if (queryable is DbSet<Domain.Entities.Positions> dbSet)
                return dbSet.FindAsync(positionId);

            var task = queryable.FirstOrDefaultAsync(q => q.PositionId == positionId);
            return new ValueTask<Domain.Entities.Positions>(task);
        }

        #endregion

    }
}
