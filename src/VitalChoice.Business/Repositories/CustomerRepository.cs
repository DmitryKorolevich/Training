using Microsoft.Data.Entity;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VitalChoice.Data.Context;
using VitalChoice.Data.Repositories.Specifics;
using VitalChoice.Ecommerce.Domain.Entities;
using VitalChoice.Ecommerce.Domain.Entities.Customers;
using VitalChoice.Infrastructure.Domain.Constants;
using VitalChoice.Infrastructure.Domain.Entities;
using VitalChoice.Infrastructure.Domain.Transfer;

namespace VitalChoice.Business.Repositories
{
    public class CustomerRepository : EcommerceRepositoryAsync<Customer>
    {
        public CustomerRepository(IDataContextAsync context) : base(context)
        {
        }

        public async Task<ICollection<string>> GetCustomerStaticFieldValuesByValue(ValuesByFieldValueFilter filter)
        {
            ICollection<string> toReturn = new List<string>();
            IQueryable<Customer> temp = (IQueryable<Customer>)this.DbSet;
            IQueryable<string> query = null;
            switch (filter.FieldName)
            {
                case "Id":
                    //BUG: should be redone on standart logic after adding normal LIKE support from EF7
                    var data = this.Context.Set<IdModel>().FromSql("SELECT DISTINCT TOP(20) [p].[Id] As Id FROM [Customers] AS [p]" +
                                        "WHERE([p].[StatusCode] <> 3) AND [p].[Id] LIKE ({0}+ '%')" +
                                        "ORDER BY [p].[Id]",filter.FieldValue).ToList();
                    return data.Select(p => p.Id.ToString()).ToList();
                case "Email":
                    temp = temp.Where(p => p.StatusCode != (int)RecordStatusCode.Deleted && p.Email.StartsWith(filter.FieldValue));
                    query = temp.Select(p => p.Email).OrderBy(p => p).Distinct();
                    break;
                default:
                    return toReturn;
            }

            if (filter.Paging == null)
            {
                filter.Paging = new Paging();
                filter.Paging.PageItemCount = BaseAppConstants.DEFAULT_AUTO_COMPLETE_TAKE_COUNT;
            }
            query = query.AsNoTracking();
            if (filter.Paging.PageIndex != 1)
            {
                query = query.Skip((filter.Paging.PageIndex - 1) * filter.Paging.PageItemCount);
            }
            toReturn = await query.Take(filter.Paging.PageItemCount).ToListAsync();
            return toReturn;
        }
    }
}
