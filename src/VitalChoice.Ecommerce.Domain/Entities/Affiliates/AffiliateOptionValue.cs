using VitalChoice.Ecommerce.Domain.Entities.Base;

namespace VitalChoice.Ecommerce.Domain.Entities.Affiliates
{
    public class AffiliateOptionValue : OptionValue<AffiliateOptionType>
    {
        public int IdAffiliate { get; set; }
    }
}