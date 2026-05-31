using System;
using System.Threading.Tasks;

namespace Application.Abstractions.Persistence
{
    public interface IUnitOfWork : IDisposable
    {
        Task<bool> Commit();
        IGenericRepository<TEntity> Repository<TEntity>() where TEntity : class;
    }
}
