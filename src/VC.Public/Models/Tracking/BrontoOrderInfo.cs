using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace VC.Public.Models.Tracking
{
    public class BrontoOrderInfo
    {
        public BrontoOrderInfo()
        {
            lineItems=new List<BrontoOrderItemInfo>();
        }

        public string url { get; set; }
        public string customerEmail { get; set; }
        public decimal subtotal { get; set; }
        public string currency { get; set; }
        public int? orderID { get; set; }
        public decimal? grandTotal { get; set; }
        public decimal? discountAmount { get; set; }
        public decimal? taxAmount { get; set; }
        public ICollection<BrontoOrderItemInfo> lineItems { get; set; }
    }
}
