using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Ecommarce.DAL.Context;
using Ecommarce.DAL.Entities;
using Ecommarce.DAL.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Ecommarce.DAL.Repositories
{
    public class GenericRepository<T> : IGenericRepository<T> where T : class
    {
        public readonly EcommarceDbContext _context;
        public readonly DbSet<T> _dbSet;
        public GenericRepository(EcommarceDbContext context)
        {
            _context = context;
            _dbSet = _context.Set<T>();
        }

        public async Task AddAsync(T entity)
        {
            await _dbSet.AddAsync(entity);
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<T>> GetAllAsync()
        {
            List<T> entities = await _dbSet.ToListAsync();
            return entities;
        }
    
        public async Task<T?> GetByIdAsync(Guid id)
        {
            return await _dbSet.FindAsync(id);
        }

        public Task Update(T entity)
        {
            _dbSet.Update(entity);
            _context.SaveChanges();
            return Task.CompletedTask;
        }
        
        public Task Delete(T entity)
        {
            _dbSet.Remove(entity);
            _context.SaveChanges();
            return Task.CompletedTask;
        }
    }
}