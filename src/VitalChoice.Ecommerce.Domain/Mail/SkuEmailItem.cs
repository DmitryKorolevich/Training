using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using VitalChoice.Ecommerce.Domain.Attributes;

namespace VitalChoice.Ecommerce.Domain.Mail
{
    public class SkuEmailItem
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

        public decimal? Price { get; set; }

		public int Quantity { get; set; }

		[Map("QTY")]
		public int PortionsCount { get; set; }

        public decimal? SubTotal { get; set; }

        [Map]
	    public string SubTitle { get; set; }
    }
}
