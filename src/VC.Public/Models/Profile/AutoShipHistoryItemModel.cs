using System;
using System.Collections.Generic;
using VitalChoice.Ecommerce.Domain.Attributes;

namespace VC.Public.Models.Profile
{
    public class AutoShipHistoryItemModel
    {
	    public AutoShipHistoryItemModel()
	    {
			ShippingDetails = new List<KeyValuePair<string, string>>();
			BillingDetails = new List<KeyValuePair<string, string>>();
			PaymentMethodDetails = new List<KeyValuePair<string, string>>();
		}

	    private string _productPageUrl;

		[Map]
		public int Id { get; set; }

		[Map]
		public string Name { get; set; }

		public string DisplayName { get; set; }

		[Map("Url")]
		public string ProductUrl
	    {
			get { return _productPageUrl; }
			set { _productPageUrl = "/product/" + value; }
		}

		[Map("AutoShipFrequency")]
		public int Frequency { get; set; }

	    public bool Active { get; set; }

        public DateTime NextDate { get; set; }

        public IList<KeyValuePair<string,string>> ShippingDetails { get; set; }

		public IList<KeyValuePair<string, string>> BillingDetails { get; set; }

		public IList<KeyValuePair<string, string>> PaymentMethodDetails { get; set; }

		[Map("Thumbnail")]
		public string IconUrl { get; set; }

		[Map("QTY")]
		public int PortionsCount { get; set; }

		[Map]
		public string SubTitle { get; set; }

		[Map]
		public string Code { get; set; }
    }
}
