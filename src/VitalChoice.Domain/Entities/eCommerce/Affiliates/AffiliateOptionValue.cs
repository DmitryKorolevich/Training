using VitalChoice.Domain.Entities.eCommerce.Base;

namespace VitalChoice.Domain.Entities.eCommerce.Affiliates
{
    public class AffiliateOptionValue : OptionValue<AffiliateOptionType>
    {
        public int IdAffiliate { get; set; }
    }
}