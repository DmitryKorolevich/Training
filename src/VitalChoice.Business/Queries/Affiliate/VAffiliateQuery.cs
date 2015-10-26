using System;
using System.Collections.Generic;
using VitalChoice.Data.Helpers;
using VitalChoice.Domain;
using VitalChoice.Domain.Constants;
using VitalChoice.Domain.Entities;
using VitalChoice.Domain.Entities.Content;
using VitalChoice.Domain.Entities.eCommerce.Affiliates;
using VitalChoice.Domain.Entities.eCommerce.Orders;
using VitalChoice.Domain.Entities.eCommerce.Products;
using VitalChoice.Domain.Entities.Logs;

namespace VitalChoice.Business.Queries.Affiliate
{
    public class VAffiliateQuery : QueryObject<VAffiliate>
    {
        public VAffiliateQuery WithId(int? id)
        {
            if (id.HasValue)
            {
                And(x => x.Id==id);
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
            And(x => x.StatusCode != RecordStatusCode.Deleted);
            return this;
        }
    }
}