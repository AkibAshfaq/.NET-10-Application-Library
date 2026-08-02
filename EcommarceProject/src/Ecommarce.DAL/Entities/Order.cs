using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Ecommarce.DAL.Entities
{
    public class Order
    {
        [Key]
        public Guid OrderId { get; set; }
        [ForeignKey("Customer")]
        public Guid CustomerId { get; set; }
        public List<OrderItem> OrderDetails { get; set; } = new List<OrderItem>();
        public decimal TotalAmount { get; set; }
        public string Status { get; set; } = "Pending";
        public virtual required Customer Customer { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}