using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VC.Public.Models.Cart;

namespace VC.Public.Models.Checkout
{
    public class ReviewOrderModel:ViewCartModel
    {
	    public ReviewOrderModel()
	    {
			BillToAddress = new List<KeyValuePair<string, string>>();
            CreditCardDetails = new List<KeyValuePair<string, string>>();
            ShipToAddress = new List<KeyValuePair<string, string>>();
		}

	    public IList<KeyValuePair<string, string>> BillToAddress { get; set; }

        public IList<KeyValuePair<string, string>> CreditCardDetails { get; set; }

        public IList<KeyValuePair<string, string>> ShipToAddress { get; set; }
    }
}
