using System.ComponentModel.DataAnnotations;
using VitalChoice.Ecommerce.Domain.Attributes;
using VitalChoice.Infrastructure.Domain.Constants;

namespace VC.Public.Models.Auth
{
    public class RegisterWholesaleAccountModel : RegisterAccountModel
    {
		[Display(Name = "Trade Class")]
		[Map]
		public int? TradeClass { get; set; }

		[Display(Name = "Websites where you will promote our products(1 per line, max 4)")]
		[Map]
		public string PromotingWebsites { get; set; }
	}
}
