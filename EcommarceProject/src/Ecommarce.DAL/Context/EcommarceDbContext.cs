using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace Ecommarce.DAL.Context
{
    public class EcommarceDbContext: DbContext
    {
        public EcommarceDbContext(DbContextOptions<EcommarceDbContext> options) : base(options)
        {
            
        }

        public DbSet<Entities.Customer> Customers => Set<Entities.Customer>();
        public DbSet<Entities.Order> Orders => Set<Entities.Order>();
        public DbSet<Entities.Product> Products => Set<Entities.Product>();
    }
}