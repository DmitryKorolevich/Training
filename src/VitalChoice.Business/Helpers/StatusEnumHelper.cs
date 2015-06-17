using System.Collections.Generic;
using VitalChoice.Domain.Entities;
using VitalChoice.Domain.Entities.Content;
using VitalChoice.Domain.Entities.eCommerce.Products;

namespace VitalChoice.Core.Infrastructure.Helpers
{
    public static class StatusEnumHelper
    {
        public static string GetContentItemStatusName(CustomerTypeCode code, RecordStatusCode status)
        {
            string toReturn = null;
            if (status == RecordStatusCode.NotActive)
            {
                toReturn = "Draft";
            }
            if (status == RecordStatusCode.Active)
            {
                if (code == CustomerTypeCode.All)
                {
                    toReturn = "Published - All";
                }
                if (code == CustomerTypeCode.Wholesale)
                {
                    toReturn = "Published - Wholesale Only";
                }
            }
            return toReturn;
        }

        public static Dictionary<string,string> GetContentItemStatusNames()
        {
            Dictionary<string, string> toReturn = new Dictionary<string, string>
            {
                {$"{(int) RecordStatusCode.NotActive}:{(int) CustomerTypeCode.All}", "Draft"},
                {$"{(int) RecordStatusCode.Active}:{(int) CustomerTypeCode.All}", "Published - All"},
                {$"{(int) RecordStatusCode.Active}:{(int) CustomerTypeCode.Wholesale}", "Published - Wholesale Only"}
            };
            return toReturn;
        }

        public static Dictionary<string, string> GetProductCategoryStatusNames()
        {
            Dictionary<string, string> toReturn = new Dictionary<string, string>
            {
                {$"{(int) RecordStatusCode.NotActive}:{(int) CustomerTypeCode.All}", "Hide within storefront"},
                {$"{(int) RecordStatusCode.Active}:{(int) CustomerTypeCode.All}", "Active - All Customers"},
                {
                    $"{(int) RecordStatusCode.Active}:{(int) CustomerTypeCode.Wholesale}",
                    "Active - Wholesale Customers Only"
                }
            };
            return toReturn;
        }

        public static string GetGCTypeName(GCType type)
        {
            string toReturn = null;
            if (type == GCType.ManualGC)
            {
                toReturn = "Manually Created Gift Certificate";
            }
            return toReturn;
        }

        public static Dictionary<int, string> GetGCTypeNames()
        {
            Dictionary<int, string> toReturn = new Dictionary<int, string>
            {
                {(int) GCType.ManualGC, "Manually Created Gift Certificate"}
            };
            return toReturn;
        }

        public static Dictionary<int, string> GetRecordStatuses()
        {
            Dictionary<int, string> toReturn = new Dictionary<int, string>
            {
                {(int) RecordStatusCode.Active, "Active"},
                {(int) RecordStatusCode.NotActive, "Not Active"},
                {(int) RecordStatusCode.Deleted, "Deleted"}
            };
            return toReturn;
        }

        public static Dictionary<int, string> GetProductTypes()
        {
            Dictionary<int, string> toReturn = new Dictionary<int, string>
            {
                {(int) ProductType.Perishable, "Perishable"},
                {(int) ProductType.NonPerishable, "Non Perishable"},
                {(int) ProductType.Gc, "Gift Certificate"},
                {(int) ProductType.EGс, "E Gift Certificate"}
            };
            return toReturn;
        }
    }
}