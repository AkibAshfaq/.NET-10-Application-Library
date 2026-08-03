using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Ecommarce.DAL.Entities;

namespace Ecommarce.DAL.Repositories.Interfaces
{
    public interface IGenericRepository<T>
    {
       Task<IEnumerable<T>> GetAllAsync();
       Task<T?> GetByIdAsync(Guid id);
       Task AddAsync(T entity);
       Task Update(T entity);
       Task Delete(T entity);
    }
}