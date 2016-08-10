using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace VC.Public.Models.Tracking
{
    public class GATransactionItemInfo
    {
        public int id { get; set; }
        public string sku { get; set; }
        public string name { get; set; }
        public string category { get; set; }
        public decimal price { get; set; }
        public int quantity { get; set; }
    }
}
