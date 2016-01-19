using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VC.Public.Models.Cart;
using VitalChoice.Validation.Models;

namespace VC.Public.Models.Checkout
{
    public class ReceiptModel:BaseModel
    {
	    public ReceiptModel()
	    {
			BillToAddress = new List<KeyValuePair<string, string>>();
			ShipToAddress = new List<KeyValuePair<string, string>>();
			Skus = new List<CartSkuModel>();
		}

	    public IList<KeyValuePair<string, string>> BillToAddress { get; set; }

	    public IList<KeyValuePair<string, string>> ShipToAddress { get; set; }

		public IList<CartSkuModel> Skus { get; set; }

		public decimal SubTotal { get; set; }

		public decimal OrderTotal { get; set; }

		public decimal ShippingCost { get; set; }

		public decimal Tax { get; set; }
	}
}
