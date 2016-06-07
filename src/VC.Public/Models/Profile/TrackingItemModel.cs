using System;
using VitalChoice.Validation.Models;
using VitalChoice.Ecommerce.Domain.Entities.Orders;
using VitalChoice.Infrastructure.Domain.Transfer.Orders;

namespace VC.Public.Models.Profile
{
    public class TrackingItemModel
    {
        public string TrackingNumber { get; set; }

        public string Sku { get; set; }

        public string ServiceUrl { get; set; }
    }
}