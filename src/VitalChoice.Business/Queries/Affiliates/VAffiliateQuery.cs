using System;
using VitalChoice.Data.Helpers;
using VitalChoice.Ecommerce.Domain.Entities;
using VitalChoice.Ecommerce.Domain.Entities.Affiliates;
using VitalChoice.Infrastructure.Domain.Constants;
using VitalChoice.Infrastructure.Domain.Transfer.Affiliates;

namespace VitalChoice.Business.Queries.Affiliate
{
    public class VAffiliateQuery : QueryObject<VAffiliate>
    {
        public VAffiliateQuery WithId(int? id)
        {
            if (id.HasValue)
            {
                And(x => x.Id == id.Value);
            }
            return this;
        }

        public VAffiliateQuery WithName(string name)
        {
            if (!String.IsNullOrEmpty(name))
            {
                And(x => x.Name.Contains(name));
            }
            return this;
        }

        public VAffiliateQuery WithCompany(string company)
        {
            if (!String.IsNullOrEmpty(company))
            {
                And(x => x.Company.Contains(company));
            }
            return this;
        }

        public VAffiliateQuery WithTier(int? tier)
        {
            if (tier.HasValue)
            {
                string value = tier.Value.ToString();
                And(x => x.Tier == value);
            }
            return this;
        }

        public VAffiliateQuery NotDeleted()
        {
            And(x => x.StatusCode != AffiliateStatus.Deleted);
            return this;
        }

        public VAffiliateQuery WithAvailablePayCommision(bool value)
        {
            if (value)
            {
                And(x => x.NotPaidCommission.Amount>=AffiliateConstants.AffiliateMinPayCommisionsAmount);
            }
            return this;
        }
    }
}