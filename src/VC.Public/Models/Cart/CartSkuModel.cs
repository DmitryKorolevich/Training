using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace VC.Public.Models.Cart
{
    public class CartSkuModel
    {
		public string Code { get; set; }

		public string ProductPageUrl { get; set; }

		public string DisplayName { get; set; }

	    public string IconUrl { get; set; }

	    public bool InStock { get; set; }

	    public decimal? Price { get; set; }

	    public int Quantity { get; set; }

	    public decimal? SubTotal { get; set; }
    }
}
