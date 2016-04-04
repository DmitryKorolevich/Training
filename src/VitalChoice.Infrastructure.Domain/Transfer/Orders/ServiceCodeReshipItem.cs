using System;
using System.Collections.Generic;
using System.Linq;
using VitalChoice.Ecommerce.Domain.Entities.Customers;
using VitalChoice.Infrastructure.Domain.Dynamic;
using VitalChoice.Ecommerce.Domain.Entities.Addresses;

namespace VitalChoice.Infrastructure.Domain.Transfer.Orders
{
    public class ServiceCodeReshipItem
    {
        public int Id { get; set; }

        public DateTime DateCreated { get; set; }

        public int? IdOrderSource { get; set; }

        public DateTime? ShipDate { get; set; }

        public string Warehouse { get; set; }

        public string ShippingStateCode { get; set; }

        public string Carrier { get; set; }

        public string ShipService { get; set; }

        public decimal Total { get; set; }

        public IList<string> SkuCodes { get; set; }

        public string SkuCodesLine {
            get
            {
                var toReturn = String.Empty;
                if(SkuCodes!=null)
                {
                    for (int i = 0; i < SkuCodes.Count; i++)
                    {
                        toReturn += SkuCodes[i] + ((i==SkuCodes.Count-1) ? "" : ", ");
                    }
                }
                return toReturn;
            }
        }

        public string OrderNotes { get; set; }

        public ServiceCodeReshipItem(OrderDynamic reship, ICollection<VitalChoice.Ecommerce.Domain.Entities.Addresses.Country> countries)
        {
            if (reship != null)
            {
                Id = reship.Id;
                DateCreated = reship.DateCreated;
                IdOrderSource = reship.IdOrderSource;
                Total = reship.Total;
                var listAmount = reship.Customer.IdObjectType == (int)CustomerType.Retail
                    ? reship.Skus.Sum(p => p.Quantity * p.Sku.Price)
                    : reship.Skus.Sum(p => p.Quantity * p.Sku.WholesalePrice);
                Total -= listAmount;
                SkuCodes = reship.ReshipProblemSkus?.Select(pp => pp.Code).ToList() ?? new List<string>();
                OrderNotes = reship.SafeData.OrderNotes;
                if (reship.ShippingAddress?.IdState.HasValue==true)
                {
                    ShippingStateCode =
                        countries.SelectMany(p => p.States)
                            .FirstOrDefault(p => p.Id == reship.ShippingAddress.IdState.Value)?
                            .StateCode;
                }
            }
        }
    }
}
