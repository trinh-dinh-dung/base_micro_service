using Application.Abstractions.Persistence;
using Infrastructure.DataContext;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Infrastructure.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private Dictionary<(Type type, string name), object> _repositories;
        private readonly SopContext _context;
        private bool _disposed;

        public UnitOfWork(SopContext context)
        {
            _context = context;
        }

        public async Task<bool> Commit()
        {
            var retryCount = 3;
            while (retryCount > 0)
            {
                try
                {
                    await _context.SaveChangesAsync();
                    return true;
                }
                catch (DbUpdateConcurrencyException)
                {
                    retryCount--;
                    if (retryCount == 0)
                        return false;
                }
            }

            return true;
        }

        public IGenericRepository<TEntity> Repository<TEntity>() where TEntity : class
        {
            return (IGenericRepository<TEntity>)GetOrAddRepository(
                typeof(TEntity),
                new GenericRepository<TEntity>(_context));
        }

        private object GetOrAddRepository(Type type, object repo)
        {
            _repositories ??= new Dictionary<(Type type, string Name), object>();

            if (_repositories.TryGetValue((type, repo.GetType().FullName), out var repository))
                return repository;

            _repositories.Add((type, repo.GetType().FullName), repo);
            return repo;
        }

        public void Dispose()
        {
            if (_disposed)
                return;

            _context.Dispose();
            _disposed = true;
            GC.SuppressFinalize(this);
        }
    }
}
