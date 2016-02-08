using System;
using VitalChoice.Ecommerce.Domain.Entities;
using VitalChoice.Ecommerce.Domain.Entities.History;

namespace VitalChoice.Infrastructure.Domain.Transfer.Settings
{
    public class ObjectHistoryOrderLogListItemModel : ObjectHistoryLogListItemModel
    {
        public int? OrderStatus { get; set; }

        public int? POrderStatus { get; set; }

        public int? NPOrderStatus { get; set; }

        public ObjectHistoryOrderLogListItemModel(ObjectHistoryLogItem item) :
            base(item)
        {
            if (!string.IsNullOrEmpty(item.OptionalData))
            {
                var parts = item.OptionalData.Split(',');
                foreach (var part in parts)
                {
                    if (part.StartsWith("All:"))
                    {
                        OrderStatus = GetOrderStatus(part, "All:");
                    }
                    if (part.StartsWith("P:"))
                    {
                        POrderStatus = GetOrderStatus(part, "P:");
                    }
                    if (part.StartsWith("NP:"))
                    {
                        NPOrderStatus = GetOrderStatus(part, "NP:");
                    }
                }
            }
        }

        private int? GetOrderStatus(string data, string tag)
        {
            int? toReturn = null;
            if (data.Contains(tag))
            {
                string sStatus = data.Replace(tag, "");
                if (!string.IsNullOrEmpty(sStatus))
                {
                    int res;
                    if (Int32.TryParse(sStatus, out res))
                        toReturn = res;
                }
            }
            return toReturn;
        }
    }
}