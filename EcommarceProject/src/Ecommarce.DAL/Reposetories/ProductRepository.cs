using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Ecommarce.DAL.Context;
using Ecommarce.DAL.Entities;
using Ecommarce.DAL.Repositories;
using Ecommarce.DAL.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Ecommarce.DAL.Reposetories
{
    public class ProductRepository : GenericRepository<Product>, IProductRepository
    {
        public ProductRepository(EcommarceDbContext context) : base(context) {}

        public async Task<IEnumerable<Product>> GetByCategoryAsync(Guid categoryId)
        {
            List<Product> products = await _dbSet.Where(p => p.CategoryId == categoryId).ToListAsync();
            return products;
        }

        public async Task<IEnumerable<Product>> SearchByNameAsync(string name)
        {
            List<Product> products = await _dbSet.Where(p => p.Name.Contains(name)).ToListAsync();
            return products;
        }
    }
}