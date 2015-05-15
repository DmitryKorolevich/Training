using System;
using System.Collections.Generic;
using VitalChoice.Domain.Entities.Content;

namespace VitalChoice.Business.Helpers
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
            Dictionary<string, string> toReturn = new Dictionary<string, string>();
            toReturn.Add(String.Format("{0}:{1}", (int)RecordStatusCode.NotActive, (int)CustomerTypeCode.All), "Draft");
            toReturn.Add(String.Format("{0}:{1}", (int)RecordStatusCode.Active, (int)CustomerTypeCode.All), "Published - All");
            toReturn.Add(String.Format("{0}:{1}", (int)RecordStatusCode.Active, (int)CustomerTypeCode.Wholesale), "Published - Wholesale Only");
            return toReturn;
        }

        public static Dictionary<string, string> GetProductCategoryStatusNames()
        {
            Dictionary<string, string> toReturn = new Dictionary<string, string>();
            toReturn.Add(String.Format("{0}:{1}", (int)RecordStatusCode.NotActive, (int)CustomerTypeCode.All), "Hide within storefront");
            toReturn.Add(String.Format("{0}:{1}", (int)RecordStatusCode.Active, (int)CustomerTypeCode.All), "Active - All Customers");
            toReturn.Add(String.Format("{0}:{1}", (int)RecordStatusCode.Active, (int)CustomerTypeCode.Wholesale), "Active - Wholesale Customers Only");
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
            Dictionary<int, string> toReturn = new Dictionary<int, string>();
            toReturn.Add((int)GCType.ManualGC, "Manually Created Gift Certificate");
            //toReturn.Add(String.Format("{0}:{1}", (int)GCType.EGC, (int)CustomerTypeCode.All), "Not set");
            //toReturn.Add(String.Format("{0}:{1}", (int)GCType.GC, (int)CustomerTypeCode.Wholesale), "Not set");
            return toReturn;
        }

        public static Dictionary<int, string> GetRecordStatuses()
        {
            Dictionary<int, string> toReturn = new Dictionary<int, string>();
            toReturn.Add((int)RecordStatusCode.Active, "Active");
            toReturn.Add((int)RecordStatusCode.NotActive, "Not Active");
            toReturn.Add((int)RecordStatusCode.Deleted, "Deleted");
            return toReturn;
        }
    }
}