using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VitalChoice.Business.Queries.Orders;
using VitalChoice.Business.Services.Dynamic;
using VitalChoice.Caching.Extensions;
using VitalChoice.Data.Context;
using VitalChoice.Data.Repositories.Specifics;
using VitalChoice.DynamicData.Helpers;
using VitalChoice.Ecommerce.Domain.Entities;
using VitalChoice.Ecommerce.Domain.Entities.Base;
using VitalChoice.Ecommerce.Domain.Entities.Customers;
using VitalChoice.Ecommerce.Domain.Entities.Orders;
using VitalChoice.Infrastructure.Context;
using VitalChoice.Infrastructure.Domain.Constants;
using VitalChoice.Infrastructure.Domain.Dynamic;
using VitalChoice.Infrastructure.Domain.Entities;
using VitalChoice.Infrastructure.Domain.Entities.Customers;
using VitalChoice.Infrastructure.Domain.Transfer;
using VitalChoice.Infrastructure.Domain.Transfer.Orders;

namespace VitalChoice.Business.Repositories
{
    public class OrderRepository : EcommerceRepositoryAsync<Order>
    {
        private readonly OrderMapper _orderMapper;

        public OrderRepository(EcommerceContext context, OrderMapper orderMapper) : base(context)
        {
            _orderMapper = orderMapper;
        }

        public async Task<List<CustomerOrderStatistic>> GetCustomerOrderStatistics(ICollection<int> ids)
        {
            var orderQuery = new OrderQuery().WithCustomerIds(ids).NotDeleted().WithActualStatusOnly();
            var query = this.DbSet.AsNoTracking().Where(orderQuery.Query());
            
            var result = await query.GroupBy(p => p.IdCustomer).Select(g => new CustomerOrderStatistic()
            {
                IdCustomer = g.Key,
                TotalOrders = g.Count(),
                FirstOrderPlaced = g.Min(p => p.DateCreated),
                LastOrderPlaced = g.Max(p => p.DateCreated),
            }).ToListAsync();

            return result;
        }

        public async Task<decimal> GetOrderWithRegionInfoAmountAsync(OrderRegionFilter filter)
        {
            VOrderWithRegionInfoItemQuery conditions = new VOrderWithRegionInfoItemQuery().WithDates(filter.From, filter.To).
                WithIdCustomerType(filter.IdCustomerType).WithIdOrderType(filter.IdOrderType).WithRegion(filter.Region).WithZip(filter.Zip);

            // ReSharper disable once PossibleNullReferenceException
            var toReturn = await (this.Context as DbContext).Set<VOrderWithRegionInfoItem>().Where(conditions.Query()).SumAsync(p => p.Total);
            return toReturn;
        }

        public async Task<ICollection<OrderForAgentReport>> GetOrdersForAgentReportAsync(DateTime from, DateTime to,
            List<int> specififcAgentIds)
        {
            OrderQuery conditions = new OrderQuery().NotDeleted().WithActualStatusOnly().WithFromDate(from).WithToDate(to).
                WithCreatedByAgentsOrWithout(specififcAgentIds);
            var orderTypeOption = _orderMapper.OptionTypes.First(p => p.Name == "OrderType");

            var orders = await this.DbSet.Include(o => o.OptionValues).Where(conditions.Query()).AsNoTracking().ToListAsync();
            // ReSharper disable once PossibleNullReferenceException
            //var orderOptionValueQuery = (this.Context as DbContext).Set<OrderOptionValue>().Where(p => p.IdOptionType == orderTypeOption.Id);

            //var result = await (from o in orderQuery
            //             join v in orderOptionValueQuery on o.Id equals v.IdOrder into grouping
            //             from d in grouping
            //             select new
            //             {
            //                 Order = o,
            //                 OrderType =d
            //             }).ToListAsync();

            return orders.Select(p => new OrderForAgentReport
            {
                Order = p,
                OrderType =
                    (SourceOrderType?) (int?)
                        MapperTypeConverter.ConvertTo<OrderOptionValue, OrderOptionType>(
                            p.OptionValues.FirstOrDefault(v => v.IdOptionType == orderTypeOption.Id),
                            (FieldType) orderTypeOption.IdFieldType)
            }).ToList();
        }
    }
}
