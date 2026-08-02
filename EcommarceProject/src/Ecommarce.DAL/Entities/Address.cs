using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Ecommarce.DAL.Entities
{
    public class Address
    {
        [Key]
        public Guid Id { get; set; }
        [ForeignKey("Customer")]
        public Guid CustomerId { get; set; }        
        public virtual required Customer Customer { get; set; }       
        public string Street { get; set; }=string.Empty;
        public string City { get; set; }=string.Empty;
        public string State { get; set; }=string.Empty;
        public string PostalCode { get; set; }=string.Empty;
        public string Country { get; set; }=string.Empty;
        public bool IsDefault { get; set; }=false;
        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}