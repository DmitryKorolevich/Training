using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace VC.Public.Models.Cart
{
    public class AutoShipModel
    {
		private string _productPageUrl;

		public int IdSchedule { get; set; }

	    public string SkuCode { get; set; }

	    public string DisplayName { get; set; }

	    public string ProductUrl {
			get { return _productPageUrl; }
			set { _productPageUrl = "/product/" + value; }
		}
    }
}
