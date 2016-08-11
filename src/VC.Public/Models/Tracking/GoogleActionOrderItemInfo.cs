using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace VC.Public.Models.Tracking
{
    public class GoogleActionOrderItemInfo
    {
        public string name { get; set; }
        public string id { get; set; }
        public decimal price { get; set; }
        public string brand { get; set; }
        public string category { get; set; }
        public string variant { get; set; }
        public int quantity { get; set; }
    }
}
