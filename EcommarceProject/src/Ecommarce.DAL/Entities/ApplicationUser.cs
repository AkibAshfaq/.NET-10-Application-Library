using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Ecommarce.DAL.Entities
{
    public class ApplicationUser
    {
        public int Id { get; set; }
        public string HashedPassword { get; set; } = string.Empty;
    }
}