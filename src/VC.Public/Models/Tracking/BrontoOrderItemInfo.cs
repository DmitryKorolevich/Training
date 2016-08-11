using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace VC.Public.Models.Tracking
{
    public class BrontoOrderItemInfo
    {
        public string sku { get; set; }
        public decimal unitPrice { get; set; }
        public string productUrl { get; set; }
        public string name { get; set; }
        public int quantity { get; set; }
        public decimal totalPrice { get; set; }
        public string imageUrl { get; set; }
    }
}
