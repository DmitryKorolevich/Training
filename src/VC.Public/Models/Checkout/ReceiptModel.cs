using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
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

		[Display(Name = "Subtotal")]
		public decimal SubTotal { get; set; }

		[Display(Name = "Order Total")]
		public decimal OrderTotal { get; set; }

		[Display(Name = "Shipping")]
		public decimal ShippingCost { get; set; }

		[Display(Name = "Tax")]
		public decimal Tax { get; set; }

		[Display(Name = "Your Order Number")]
		public string OrderNumber { get; set; }

		[Display(Name = "Order Date")]
		public DateTime OrderDate { get; set; }

		[Display(Name = "Payment Method")]
		public string PaymentMethod { get; set; }

		[Display(Name = "Transaction ID")]
		public string TransactionId { get; set; }

		[Display(Name = "Pre-Authorization Code")]
		public string PreAuthorizationCode { get; set; }

		[Display(Name = "Shipping")]
	    public string ShippingMethod { get; set; }
    }
}
