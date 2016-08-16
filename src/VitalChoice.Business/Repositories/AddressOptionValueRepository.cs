using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VitalChoice.Data.Context;
using VitalChoice.Data.Repositories.Specifics;
using VitalChoice.Ecommerce.Domain.Entities;
using VitalChoice.Ecommerce.Domain.Entities.Addresses;
using VitalChoice.Infrastructure.Context;
using VitalChoice.Infrastructure.Domain.Constants;
using VitalChoice.Infrastructure.Domain.Transfer;

namespace VitalChoice.Business.Repositories
{
    public class AddressOptionValueRepository : EcommerceRepositoryAsync<AddressOptionValue>
    {
        public AddressOptionValueRepository(EcommerceContext context) : base(context)
        {
        }

        public async Task<ICollection<string>> GetAddressFieldValuesByValue(ValuesByFieldValueFilter filter)
        {
            ICollection<string> toReturn = new List<string>();
            if (!String.IsNullOrEmpty(filter.FieldValue) && filter.FieldIds!=null &&
                filter.FieldIds.Count>0)
            {
                var context = this.Context;
                var temp = this.DbSet.Where(p => filter.FieldIds.Contains(p.IdOptionType) && p.Value.StartsWith(filter.FieldValue) &&
                    p.Address.StatusCode != (int)RecordStatusCode.Deleted);
                if(filter.IdReferencedObjectType.HasValue)
                {
                    temp = temp.Where(p=>p.Address.IdObjectType == filter.IdReferencedObjectType.Value);
                }
                var query = temp.Select(p => p.Value).OrderBy(p=>p).Distinct();
                if (filter.Paging == null)
                {
                    filter.Paging = new Paging();
                    filter.Paging.PageItemCount = BaseAppConstants.DEFAULT_AUTO_COMPLETE_TAKE_COUNT;
                }
                query = query.AsNoTracking();
                if (filter.Paging.PageIndex != 1)
                {
                    query = query.Skip((filter.Paging.PageIndex-1) * filter.Paging.PageItemCount);
                }
                toReturn = await query.Take(filter.Paging.PageItemCount).ToListAsync();
            }
            return toReturn;
        }
    }
}
