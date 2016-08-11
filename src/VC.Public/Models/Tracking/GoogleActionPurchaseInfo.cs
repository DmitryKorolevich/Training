using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace VC.Public.Models.Tracking
{
    public class GoogleActionPurchaseInfo
    {
        public GoogleActionPurchaseInfo()
        {
            products = new List<GoogleActionOrderItemInfo>();
        }

        public GoogleActionPurchaseActionInfo actionField { get; set; }
        public ICollection<GoogleActionOrderItemInfo> products { get; set; }
    }
}
