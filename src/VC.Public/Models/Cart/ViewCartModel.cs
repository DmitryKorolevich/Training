using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using VC.Public.DataAnnotations;
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
			CrossSells = new List<CartCrossSellModel>();
		}

	    public bool AutoShip { get; set; }

	    public bool ShipAsap { get; set; }

		[FutureDate(ErrorMessage = "Shipping Date should be in the future")]
        [Display(Name = "Shipping Date")]
        public DateTime? ShippingDate { get; set; }

        public string ShippingDateError { get; set; }

		[Display(Name = "Tax")]
		public decimal Tax { get; set; }

		[Display(Name = "Subtotal")]
		public decimal SubTotal { get; set; }

		[Display(Name = "Shipping")]
		public decimal? ShippingCost { get; set; }

        public decimal? FreeShipDifference { get; set; }

		[Display(Name = "Order Total")]
		public decimal OrderTotal { get; set; }

		[Display(Name = "Discount")]
		public decimal? DiscountTotal { get; set; }

		[Display(Name = "Gift Certificates")]
		public decimal? GiftCertificatesTotal { get; set; }

	    public string PromoCode { get; set; }

		public ICollection<LookupItem<ShippingUpgradeOption>> ShippingUpgradePOptions { get; set; }

		public ICollection<LookupItem<ShippingUpgradeOption>> ShippingUpgradeNPOptions { get; set; }

		public ShippingUpgradeOption? ShippingUpgradeP { get; set; }

		public ShippingUpgradeOption? ShippingUpgradeNP { get; set; }

		public IList<CartGcModel> GiftCertificateCodes { get; set; }

		public IList<CartSkuModel> Skus { get; set; }

		public IList<CartSkuModel> PromoSkus { get; set; }

	    public string DiscountMessage { get; set; }

	    public string DiscountDescription { get; set; }

	    public IList<KeyValuePair<string, string>> Messages { get; set; }

		public IList<CartCrossSellModel> CrossSells { get; set; }
	}
}
