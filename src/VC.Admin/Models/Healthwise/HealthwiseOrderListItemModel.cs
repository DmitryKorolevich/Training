﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VitalChoice.Ecommerce.Domain.Entities.Healthwise;
using VitalChoice.Infrastructure.Domain.Entities.Healthwise;

namespace VC.Admin.Models.Healthwise
{
    public class HealthwiseOrderListItemModel
    {
        public int Id { get; set; }

        public DateTime DateCreated { get; set; }

        public decimal ProductsSubtotal { get; set; }

        public decimal DiscountedSubtotal { get; set; }

        public HealthwiseOrderListItemModel(HealthwiseOrder item)
        {
            if(item!=null)
            {
                Id = item.Id;
                DateCreated = item.Order.DateCreated;
                ProductsSubtotal = item.Order.ProductsSubtotal;
                DiscountedSubtotal = item.Order.ProductsSubtotal - item.Order.DiscountTotal;
            }
        }
    }
}
