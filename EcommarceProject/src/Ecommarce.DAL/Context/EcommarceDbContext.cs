using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace Ecommarce.DAL.Context
{
    public class EcommarceDbContext: DbContext
    {
        public EcommarceDbContext(DbContextOptions<EcommarceDbContext> options) : base(options) {}

        DbSet<Entities.Customer> Customers { get; set; }
        DbSet<Entities.Order> Orders { get; set; }
        DbSet<Entities.Product> Products { get; set; }
    }
}