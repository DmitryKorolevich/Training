using System;
using System.ComponentModel.DataAnnotations;

namespace VC.Public.Models.Checkout
{
    public class ReceiptModel: ReviewOrderModel
	{
	    [Display(Name = "Your Order Number")]
		public string OrderNumber { get; set; }

		[Display(Name = "Order Date")]
		public DateTime OrderDate { get; set; }

        public bool ShowEGiftEmailForm { get; set; }

        public EGiftSendEmailModel EGiftSendEmail { get; set; }

        public string GATransactionInfo { get; set; }

        public string GAItemsInfo { get; set; }
    }
}
