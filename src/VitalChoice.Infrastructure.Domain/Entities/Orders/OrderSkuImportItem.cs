using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace VitalChoice.Infrastructure.Domain.Entities.Orders
{
    public class OrderSkuImportItem
    {
        [Display(Name = "SKU")]
        public string SKU { get; set; }

        [Display(Name = "QTY")]
        public int QTY { get; set; }
    }
}
