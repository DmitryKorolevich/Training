using System;
using System.Collections.Generic;
using VitalChoice.Ecommerce.Domain.Entities;
using VitalChoice.Ecommerce.Domain.Entities.GiftCertificates;

namespace VitalChoice.Infrastructure.Domain.Transfer.GiftCertificates
{
    public class GCStatisticModel
    {
        public int Id { get; set; }

        public int Count { get; set; }

        public decimal Total { get; set; }

        public ICollection<GCWithOrderListItemModel> Items { get; set; }

        public GCStatisticModel()
        {
            
        }
    }
}