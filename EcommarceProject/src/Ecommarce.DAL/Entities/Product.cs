using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Ecommarce.DAL.Entities
{
    public class Product
    {
        [Key]
        public Guid ProductId { get; set; }
        [Required]
        public string Name { get; set; }= "Product Name";
        [Required]
        public string Description { get; set; } = "No Product Description";
        [Required]
        public decimal Price { get; set; }
        [Required]
        public Guid CategoryId { get; set; }
        [Required]
        public string ImageUrl { get; set; } = "No Image";
        [Required]
        public string Brand { get; set; } = "No Brand";
        [Required]
        public string Model { get; set; } = "No Model";
        [Required]
        public int StockQuantity { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}