using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace VC.Public.Models.Affiliate
{
    public class OrderPaymentLineModel
    {
        public DateTime DateCreated { get; set; }

        public int IdOrder { get; set; }

        public decimal ProductTotal { get; set; }

        public decimal OrderTotal { get; set; }

        public decimal Shipping { get; set; }

        public decimal Tax { get; set; }

        public decimal Commission { get; set; }

	    public bool NewCustomerOrder { get; set; }
    }
}
