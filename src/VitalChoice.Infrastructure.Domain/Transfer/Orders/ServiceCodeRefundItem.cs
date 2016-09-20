using System;
using System.Collections.Generic;
using System.Linq;
using VitalChoice.Infrastructure.Domain.Dynamic;

namespace VitalChoice.Infrastructure.Domain.Transfer.Orders
{
    public class ServiceCodeRefundItem
    {
        public int Id { get; set; }

        public DateTime DateCreated { get; set; }

        public int? IdOrderSource { get; set; }

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

        public string Warehouse { get; set; }

        public string PWarehouse { get; set; }

        public string NPWarehouse { get; set; }

        public DateTime? OrderSourceDateCreated { get; set; }

        public ServiceCodeRefundItem(OrderRefundDynamic refund)
        {
            if (refund != null)
            {
                Id = refund.Id;
                DateCreated = refund.DateCreated;
                IdOrderSource = refund.IdOrderSource;
                Total = -refund.Total;
                SkuCodes = refund.RefundSkus?.Select(pp => pp.Sku?.Code).ToList() ?? new List<string>();
                OrderNotes = refund.SafeData.ServiceCodeNotes;
            }
        }
    }
}
