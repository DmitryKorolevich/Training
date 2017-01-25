using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace VC.Public.Models.Checkout
{
    public class MultipleReceiptModel
	{
	    public MultipleReceiptModel()
	    {
            Receipts =new List<ReceiptModel>();
        }

	    public IList<ReceiptModel> Receipts { get; set; }

        public bool ShowEGiftEmailForm { get; set; }

        public EGiftSendEmailModel EGiftSendEmail { get; set; }
    }
}
