using System.Linq;
using VitalChoice.Data.Helpers;
using VitalChoice.Domain.Entities;
using VitalChoice.Domain.Entities.eCommerce.Addresses;

namespace VitalChoice.Business.Queries.Affiliate
{
    public class AffiliateQuery : QueryObject<Domain.Entities.eCommerce.Affiliates.Affiliate>
    {
	    public AffiliateQuery NotDeleted()
	    {
			Add(x=>x.StatusCode != (int)RecordStatusCode.Deleted);

		    return this;
	    }

		public AffiliateQuery Excluding(int? id)
		{
			if (id.HasValue && id > 0)
				Add(p => p.Id != id.Value);
			return this;
		}

		public AffiliateQuery WithEmail(string email)
		{
			if (!string.IsNullOrEmpty(email))
			{
				Add(p => p.Email == email);
			}
			return this;
		}

		public AffiliateQuery WithId(string text)
		{
			int intValue;
			if (int.TryParse(text, out intValue))
			{
				Add(x => x.Id == intValue);
			}
			return this;
		}
	}
}
