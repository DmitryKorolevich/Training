using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VitalChoice.Ecommerce.Domain.Attributes;

namespace VC.Public.Models.Cart
{
    public class CartSkuModel
    {
        [Map]
		public string Code { get; set; }

        [Map("Url")]
        public string ProductPageUrl { get; set; }

        [Map("Name")]
		public string DisplayName { get; set; }

        [Map("Thumbnail")]
	    public string IconUrl { get; set; }

	    public bool InStock { get; set; }

	    public decimal? Price { get; set; }

	    public int Quantity { get; set; }

	    public decimal? SubTotal { get; set; }
    }
}
