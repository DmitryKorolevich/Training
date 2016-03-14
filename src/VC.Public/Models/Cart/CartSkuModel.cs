using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using VitalChoice.Ecommerce.Domain.Attributes;

namespace VC.Public.Models.Cart
{
    public class CartSkuModel
    {
        private string _productPageUrl;

        [Map]
		public string Code { get; set; }

        [Map("Url")]
        public string ProductPageUrl
        {
            get { return _productPageUrl; }
            set { _productPageUrl = "/product/" + value; }
        }

        [Map("Name")]
        public string DisplayName { get; set; }

        [Map("Thumbnail")]
        public string IconUrl { get; set; }

	    public bool InStock { get; set; }

	    public decimal? Price { get; set; }

		[Range(0, int.MaxValue, ErrorMessage = "Quantity must be greater than zero")]
		public int Quantity { get; set; }

	    public decimal? SubTotal { get; set; }

		[Map("QTY")]
		public int PortionsCount { get; set; }

		[Map]
	    public string SubTitle { get; set; }

        public ICollection<string> GeneratedGCCodes { get; set; }
    }
}
