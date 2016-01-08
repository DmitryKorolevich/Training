using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VitalChoice.Infrastructure.Domain.Transfer.Shipping;

namespace VC.Public.Models.Cart
{
    public class ViewCartModel
    {
	    public IList<CartSkuModel> Skus { get; set; }

		public bool ShipAsap { get; set; }

	    public DateTime? ShippingDate { get; set; }

	    public decimal SubTotal { get; set; }

	    public ShippingUpgradeOption? UpgradeOption { get; set; }

	    public decimal ShippingCost { get; set; }

	    public decimal OrderTotal { get; set; }

	    public string PromoCode { get; set; }

	    public string GiftCertificateCode { get; set; }
    }
}
