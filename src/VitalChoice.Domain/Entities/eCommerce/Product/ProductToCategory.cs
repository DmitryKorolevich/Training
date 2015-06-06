using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace VitalChoice.Domain.Entities.eCommerce.Product
{
    public class ProductToCategory : Entity
    {
        public int IdCategory { get; set; }

        public int IdProduct { get; set; }
    }
}
