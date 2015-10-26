using System;
using System.Collections.Generic;
using VitalChoice.Domain.Entities.eCommerce.Base;
using VitalChoice.Domain.Entities.eCommerce.Payment;
using VitalChoice.Domain.Entities.eCommerce.Users;
using VitalChoice.Domain.Entities.Settings;

namespace VitalChoice.Domain.Entities.eCommerce.Affiliates
{
    public class Affiliate : DynamicDataEntity<AffiliateOptionValue, AffiliateOptionType>
	{
	    public Affiliate()
	    {
        }

        public User User { get; set; }

        public string Name{ get; set; }

	    public decimal MyAppBalance { get; set; }

	    public decimal CommissionFirst { get; set; }

	    public decimal CommissionAll { get; set; }

        public int IdCountry { get; set; }

        public int? IdState { get; set; }

        public string County { get; set; }

        public string Email { get; set; }
    }
}
