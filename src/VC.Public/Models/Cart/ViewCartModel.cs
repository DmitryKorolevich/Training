using System;
using System.Collections.Generic;
using VitalChoice.Ecommerce.Domain.Transfer;
using VitalChoice.Infrastructure.Domain.Transfer.Shipping;

namespace VC.Public.Models.Cart
{
    public class ViewCartModel
    {
	    public ViewCartModel()
	    {
		    Skus = new List<CartSkuModel>();
		    PromoSkus = new List<CartSkuModel>();
			GiftCertificateCodes = new List<CartGcModel>();
			ShippingUpgradePOptions = new List<LookupItem<ShippingUpgradeOption>>();
			ShippingUpgradeNPOptions = new List<LookupItem<ShippingUpgradeOption>>();
			ShipAsap = true;
	    }

	    public bool ShipAsap { get; set; }

	    public DateTime? ShippingDate { get; set; }

	    public decimal SubTotal { get; set; }

	    public decimal? ShippingCost { get; set; }

	    public decimal OrderTotal { get; set; }

	    public decimal? DiscountTotal { get; set; }

	    public decimal? GiftCertificatesTotal { get; set; }

	    public string PromoCode { get; set; }

		public ICollection<LookupItem<ShippingUpgradeOption>> ShippingUpgradePOptions { get; set; }

		public ICollection<LookupItem<ShippingUpgradeOption>> ShippingUpgradeNPOptions { get; set; }

		public ShippingUpgradeOption? ShippingUpgradeP { get; set; }

		public ShippingUpgradeOption? ShippingUpgradeNP { get; set; }

		public IList<CartGcModel> GiftCertificateCodes { get; set; }

		public IList<CartSkuModel> Skus { get; set; }

		public IList<CartSkuModel> PromoSkus { get; set; }
	}
}
