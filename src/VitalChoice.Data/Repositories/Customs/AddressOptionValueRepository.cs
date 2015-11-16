using Microsoft.Data.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VitalChoice.Data.DataContext;
using VitalChoice.Data.Repositories.Specifics;
using VitalChoice.Domain.Constants;
using VitalChoice.Domain.Entities;
using VitalChoice.Domain.Entities.eCommerce.Addresses;
using VitalChoice.Domain.Entities.eCommerce.Products;
using VitalChoice.Domain.Transfer.Base;
using VitalChoice.Domain.Transfer;
using VitalChoice.Domain.Transfer.Products;

namespace VitalChoice.Data.Repositories.Customs
{
    public class AddressOptionValueRepository : EcommerceRepositoryAsync<AddressOptionValue>
    {
        public AddressOptionValueRepository(IDataContextAsync context) : base(context)
        {
        }

        public async Task<ICollection<string>> GetAddressFieldValuesByValue(ValuesByFieldValueFilter filter)
        {
            ICollection<string> toReturn = new List<string>();
            if (!String.IsNullOrEmpty(filter.FieldValue) && filter.FieldIds!=null &&
                filter.FieldIds.Count>0)
            {
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
