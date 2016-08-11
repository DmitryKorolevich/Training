using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace VC.Public.Models.Tracking
{
    public class GoogleActionCheckoutInfo
    {
        public GoogleActionCheckoutInfo()
        {
            products=new List<GoogleActionOrderItemInfo>();
        }

        public GoogleActionCheckoutStepInfo actionField { get; set; }
        public ICollection<GoogleActionOrderItemInfo> products { get; set; }
    }
}
