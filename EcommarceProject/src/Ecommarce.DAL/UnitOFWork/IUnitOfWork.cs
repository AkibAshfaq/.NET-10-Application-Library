using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Ecommarce.DAL.Entities;
using Ecommarce.DAL.Repositories.Interfaces;

namespace Ecommarce.DAL.UnitOFWork
{
    public interface IUnitOfWork: IDisposable
    {
        IProductRepository Products { get; }
        IGenericRepository<Category> Categories { get; }
        IGenericRepository<Order> Orders { get; }
        IGenericRepository<OrderItem> OrderItems { get; }
        IGenericRepository<Customer> Customers { get; }
        IGenericRepository<Address> Addresses { get; }
        IGenericRepository<ApplicationUser> ApplicationUsers { get; }
        Task<int> SaveChangesAsync();
    }
}