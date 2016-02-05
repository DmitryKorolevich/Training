using System;
using VitalChoice.Ecommerce.Domain.Entities;
using VitalChoice.Ecommerce.Domain.Entities.History;

namespace VitalChoice.Infrastructure.Domain.Transfer.Settings
{
    public class ObjectHistoryOrderLogListItemModel: ObjectHistoryLogListItemModel
    {
        public int? IdOrderStatus { get; set; }

        public int? IdPOrderStatus { get; set; }

        public int? IdNPOrderStatus { get; set; }

        public ObjectHistoryOrderLogListItemModel(ObjectHistoryLogItem item) :
            base(item)
        {
            if(!string.IsNullOrEmpty(item.OptionalData))
            {
                var parts = item.OptionalData.Split(',');
                foreach(var part in parts)
                {
                    IdOrderStatus = GetOrderStatus(item.OptionalData, "All:");
                    IdPOrderStatus = GetOrderStatus(item.OptionalData, "P:");
                    IdNPOrderStatus = GetOrderStatus(item.OptionalData, "NP:");
                }
            }
        }

        private int? GetOrderStatus(string data,string tag)
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