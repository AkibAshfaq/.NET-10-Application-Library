using System;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Ecommarce.DAL.Entities
{
    public class Customer
    {
        [Key]
        public Guid CustomerId { get; set; }
        [Required]
        public string UserName { get; set; }=string.Empty;
        public string FirstName { get; set; }=string.Empty;
        public string LastName { get; set; }=string.Empty;
        [Required]
        public string Email { get; set; }=string.Empty;
        [Required]
        public string hashedPassword { get; set; }=string.Empty;
        public string PhoneNumber { get; set; }=string.Empty;
        public List<Address> Addresses { get; set; }=new List<Address>();
        public List<Order> Orders { get; set; }=new List<Order>();
        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}