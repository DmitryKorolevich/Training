using System;
using System.Collections.Generic;
using VitalChoice.Ecommerce.Domain.Entities.Base;

namespace VitalChoice.Ecommerce.Domain.Entities.Products
{
    public class SkuOOSHistoryItem : Entity
    {
        public int IdSku { get; set; }

        public Sku Sku { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime? EndDate { get; set; }

        public bool IsCurrent { get; set; }
    }
}