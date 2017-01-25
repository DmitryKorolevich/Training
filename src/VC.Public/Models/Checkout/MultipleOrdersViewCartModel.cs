using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using VC.Public.DataAnnotations;
using VC.Public.Models.Cart;
using VitalChoice.Ecommerce.Domain.Transfer;
using VitalChoice.Infrastructure.Domain.Transfer.Shipping;
using VitalChoice.Validation.Models;

namespace VC.Public.Models.Checkout
{
    public class MultipleOrdersViewCartModel : ViewCartModel
    {
	    public MultipleOrdersViewCartModel() : base()
	    {
		}

		[FutureDate(Disabled = true)]
        [DirectLocalized("Shipping Date")]
        public override DateTime? ShippingDate { get; set; }
	}
}
