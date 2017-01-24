using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VC.Public.Models.Cart;

namespace VC.Public.Models.Checkout
{
    public class ReviewUpdateOrderModel
    {
	    public ReviewUpdateOrderModel()
	    {
			BillToAddress = new List<KeyValuePair<string, string>>();
            CreditCardDetails = new List<KeyValuePair<string, string>>();
            ShipToAddress = new List<KeyValuePair<string, string>>();
		}

        public bool Main { get; set; }

        public string Name { get; set; }

        public ViewCartModel OrderModel { get; set; }

	    public IList<KeyValuePair<string, string>> BillToAddress { get; set; }

        public IList<KeyValuePair<string, string>> CreditCardDetails { get; set; }

        public IList<KeyValuePair<string, string>> ShipToAddress { get; set; }

        public string ShipToFirstName { get; set; }

        public string ShipToLastName { get; set; }

        public string ShipToAddress1 { get; set; }

        public string DeliveryInstructions { get; set; }

        public string GiftMessage { get; set; }
    }
}
