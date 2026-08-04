using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Ecommarce.DAL.Context;
using Ecommarce.DAL.Entities;
using Ecommarce.DAL.Reposetories;
using Ecommarce.DAL.Repositories;
using Ecommarce.DAL.Repositories.Interfaces;

namespace Ecommarce.DAL.UnitOFWork
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly EcommarceDbContext _context;
        public IProductRepository Products { get; }
        public IGenericRepository<Category> Categories { get; }
        public IGenericRepository<Order> Orders { get; }
        public IGenericRepository<OrderItem> OrderItems { get; }
        public IGenericRepository<Customer> Customers { get; }
        public IGenericRepository<Address> Addresses { get; }
        public IGenericRepository<ApplicationUser> ApplicationUsers { get; }

        public UnitOfWork(EcommarceDbContext context)
        {
            _context = context;
            Products = new ProductRepository(_context);
            Categories = new GenericRepository<Category>(_context);
            Orders = new GenericRepository<Order>(_context);
            OrderItems = new GenericRepository<OrderItem>(_context);
            Customers = new GenericRepository<Customer>(_context);
            Addresses = new GenericRepository<Address>(_context);
            ApplicationUsers = new GenericRepository<ApplicationUser>(_context);
        }

        public async Task<int> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync();
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}