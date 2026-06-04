using System;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Abstractions.Persistence
{
    public interface IUnitOfWork : IDisposable
    {
        Task<bool> Commit(CancellationToken cancellationToken = default);
        IGenericRepository<TEntity> Repository<TEntity>() where TEntity : class;
    }
}
