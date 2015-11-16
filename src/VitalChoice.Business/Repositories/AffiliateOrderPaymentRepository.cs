﻿using Microsoft.Data.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VitalChoice.Data.DataContext;
using VitalChoice.Data.Repositories.Specifics;
using VitalChoice.Data.Extensions;
using System.Linq.Expressions;
using VitalChoice.Ecommerce.Domain.Entities;
using VitalChoice.Ecommerce.Domain.Entities.Affiliates;
using VitalChoice.Ecommerce.Domain.Entities.Orders;
using VitalChoice.Infrastructure.Domain.Entities;
using VitalChoice.Infrastructure.Domain.Transfer.Affiliates;

namespace VitalChoice.Data.Repositories.Customs
{
    public class AffiliateOrderPaymentRepository : EcommerceRepositoryAsync<AffiliateOrderPayment>
    {
        public AffiliateOrderPaymentRepository(
            IDataContextAsync context) : base(context)
		{
        }

        public async Task<Dictionary<int,int>> GetAffiliateOrdersInCustomers(int idAffiliate,ICollection<int> customerIds)
        {
            Expression<Func<AffiliateOrderPayment, bool>> filter = (p => p.IdAffiliate == idAffiliate && p.Order.StatusCode != (int)RecordStatusCode.Deleted &&
                (p.Order.OrderStatus == OrderStatus.Processed || p.Order.OrderStatus == OrderStatus.Shipped ||
                p.Order.OrderStatus == OrderStatus.Exported));
            foreach (var id in customerIds)
            {
                filter = filter.And((p => p.Order.IdCustomer == id));
            }
            var query = this.DbSet.Include(p => p.Order)/*.AsExpandable()*/.Where(filter);

            //var groups = await query.GroupBy(p => p.Order.IdCustomer).Select(g => new
            //{
            //    IdCustomer = g.Key,
            //    Count = g.Count(),
            //}).AsNoTracking().ToListAsync();
            //TODO - redone to group by in DB(not working with navigation properties now)
            var items = await query.AsNoTracking().ToListAsync();
            var groups = items.GroupBy(p => p.Order.IdCustomer).Select(g => new
            {
                IdCustomer = g.Key,
                Count = g.Count(),
            });
            var toReturn = groups.ToDictionary(p => p.IdCustomer, x => x.Count);

            return toReturn;
        }

        public async Task<int> GetEngangedAffiliatesCount()
        {
            var result = await Context.Set<CountModel>().FromSql("[dbo].[SPGetEngangedAffiliatesCount]").FirstOrDefaultAsync();
            return result.Count;
        }

        public async Task<ICollection<AffiliateSummaryReportModel>> GetAffiliatesSummaryReport(DateTime from,DateTime to)
        {
            var toReturn = await Context.Set<AffiliateSummaryReportModel>().FromSql("[dbo].[SPGetAffiliatesSummaryReport] @from={0}, @to={1}",from,to).ToListAsync();
            return toReturn;
        }
    }
}