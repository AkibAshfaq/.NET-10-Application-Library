using System;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Ecommarce.DAL.Entities
{
    public class Customer
    {
        public int Id { get; set; }
        public string UserId { get; set; }=string.Empty;
        public ApplicationUser ApplicationUser { get; set; }=null!;
        public string FullName { get; set; }=string.Empty;
        public string PhoneNumber { get; set; }=string.Empty;
        public List<Address> Addresses { get; set; }=new List<Address>();
        public List<Order> Orders { get; set; }=new List<Order>();
        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}