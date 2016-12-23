using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using VitalChoice.Data.Context;
using VitalChoice.Data.Repositories.Specifics;
using VitalChoice.Ecommerce.Domain;
using VitalChoice.Ecommerce.Domain.Options;
using VitalChoice.Ecommerce.Domain.Transfer;
using VitalChoice.Infrastructure.Context;
using VitalChoice.Infrastructure.Domain.Entities;
using VitalChoice.Infrastructure.Domain.Entities.Customers;
using VitalChoice.Infrastructure.Domain.Entities.InventorySkus;
using VitalChoice.Infrastructure.Domain.Entities.Orders;
using VitalChoice.Infrastructure.Domain.Entities.Products;
using VitalChoice.Infrastructure.Domain.Entities.Reports;
using VitalChoice.Infrastructure.Domain.Transfer.InventorySkus;
using VitalChoice.Infrastructure.Domain.Transfer.Orders;
using VitalChoice.Infrastructure.Domain.Transfer.Products;
using VitalChoice.Infrastructure.Domain.Transfer.Reports;

namespace VitalChoice.Business.Repositories
{
    public class SpEcommerceRepository : IDisposable
    {
        private readonly Lazy<EcommerceContext> _context;

        public SpEcommerceRepository(IOptions<AppOptionsBase> appOptions, DbContextOptions<EcommerceContext> contextOptions)
        {
            _context = new Lazy<EcommerceContext>(() =>
            {
                var context = new EcommerceContext(appOptions, contextOptions);
                context.Database.SetCommandTimeout(300);
                return context;
            });
        }

        public async Task<ICollection<ProductCategoryStatisticItem>> GetProductCategoryStatisticAsync(DateTime from, DateTime to)
        {
            var toReturn =
                await
                    _context.Value.Set<ProductCategoryStatisticItem>()
                        .FromSql("[dbo].[SPGetProductCategoryStatistic] @from={0}, @to={1}", from, to)
                        .ToListAsync();
            return toReturn;
        }

        public async Task<ICollection<SkusInProductCategoryStatisticItem>> GetSkusInProductCategoryStatisticAsync(DateTime from, DateTime to,
            int idCategory)
        {
            var toReturn =
                await
                    _context.Value.Set<SkusInProductCategoryStatisticItem>()
                        .FromSql("[dbo].[SPGetSkusInProductCategoryStatistic] @from={0}, @to={1}, @idcategory={2}", from, to, idCategory)
                        .ToListAsync();
            return toReturn;
        }

        public async Task<ICollection<OrdersRegionStatisticItem>> GetOrdersRegionStatisticAsync(OrderRegionFilter filter)
        {
            var toReturn =
                await
                    _context.Value.Set<OrdersRegionStatisticItem>()
                        .FromSql("[dbo].[SPGetOrdersRegionStatistic] @from={0}, @to={1}, @IdCustomerType={2}, @IdOrderType={3}",
                            filter.From, filter.To, filter.IdCustomerType, filter.IdOrderType).ToListAsync();
            return toReturn;
        }

        public async Task<ICollection<OrdersZipStatisticItem>> GetOrdersZipStatisticAsync(OrderRegionFilter filter)
        {
            var toReturn =
                await
                    _context.Value.Set<OrdersZipStatisticItem>()
                        .FromSql("[dbo].[SPGetOrdersZipStatistic] @from={0}, @to={1}, @IdCustomerType={2}, @IdOrderType={3}",
                            filter.From, filter.To, filter.IdCustomerType, filter.IdOrderType).ToListAsync();
            return toReturn;
        }

        public async Task<ICollection<InventorySkuUsageRawReportItem>> GetInventorySkusUsageReportAsync(InventorySkuUsageReportFilter filter)
        {
            string sSkuIds = null;
            if (filter.SkuIds != null && filter.SkuIds.Count > 0)
            {
                sSkuIds = string.Empty;
                for (int i = 0; i < filter.SkuIds.Count; i++)
                {
                    sSkuIds += filter.SkuIds[i];
                    if (i != filter.SkuIds.Count - 1)
                    {
                        sSkuIds += ",";
                    }
                }
            }
            string sInvSkuIds = null;
            if (filter.InvSkuIds != null && filter.InvSkuIds.Count > 0)
            {
                sInvSkuIds = string.Empty;
                for (int i = 0; i < filter.InvSkuIds.Count; i++)
                {
                    sInvSkuIds += filter.InvSkuIds[i];
                    if (i != filter.InvSkuIds.Count - 1)
                    {
                        sInvSkuIds += ",";
                    }
                }
            }

            var toReturn =
                await
                    _context.Value.Set<InventorySkuUsageRawReportItem>()
                        .FromSql("[dbo].[SPGetInventorySkusUsageReport] @from={0}, @to={1}, @skus={2}, @invskus={3}",
                            filter.From, filter.To, sSkuIds, sInvSkuIds).ToListAsync();
            return toReturn;
        }

        public async Task<ICollection<InventoriesSummaryUsageRawReportItem>> GetInventoriesSummaryUsageReportAsync(
            InventoriesSummaryUsageReportFilter filter)
        {
            string sIdsInvCat = null;
            if (filter.IdsInvCat != null && filter.IdsInvCat.Count > 0)
            {
                sIdsInvCat = string.Empty;
                for (int i = 0; i < filter.IdsInvCat.Count; i++)
                {
                    sIdsInvCat += filter.IdsInvCat[i];
                    if (i != filter.IdsInvCat.Count - 1)
                    {
                        sIdsInvCat += ",";
                    }
                }
            }

            var toReturn = await _context.Value.Set<InventoriesSummaryUsageRawReportItem>().FromSql(
                "[dbo].[SPGetInventoriesSummaryUsageReport] @from={0}, @to={1}, @sku={2}, @invsku={3}, " +
                "@assemble={4}, @idsinvcat={5}, @shipdate={6}, @frequency={7}",
                filter.From, filter.To, filter.Sku, filter.InvSku,
                filter.Assemble, sIdsInvCat, filter.ShipDate, (int) filter.FrequencyType).ToListAsync();
            return toReturn;
        }

        public async Task<ICollection<int>> GetOrderIdsForWholesaleDropShipReportAsync(WholesaleDropShipReportFilter filter)
        {
            if (string.IsNullOrEmpty(filter.CustomerCompany))
                filter.CustomerCompany = null;
            if (string.IsNullOrEmpty(filter.CustomerFirstName))
                filter.CustomerFirstName = null;
            if (string.IsNullOrEmpty(filter.CustomerLastName))
                filter.CustomerLastName = null;
            if (string.IsNullOrEmpty(filter.ShipFirstName))
                filter.ShipFirstName = null;
            if (string.IsNullOrEmpty(filter.ShipLastName))
                filter.ShipLastName = null;
            if (string.IsNullOrEmpty(filter.ShippingIdConfirmation))
                filter.ShippingIdConfirmation = null;

            var toReturn = await _context.Value.Set<IdModel>().FromSql
            ("[dbo].[SPGetOrderIdsForWholesaleDropShipReport] @from={0}, @to={1}, @shipfrom={2}, @shipto={3}," +
             " @idcustomertype={4}, @idtradeclass={5}, @customerfirstname={6}, @customerlastname={7}, @shipfirstname={8}," +
             " @shiplastname={9}, @shipidconfirm={10}, @idorder={11}, @ponumber={12}, @customercompany={13}, @pageindex={14}, @pagesize={15}",
                filter.From, filter.To, filter.ShipFrom, filter.ShipTo,
                filter.IdCustomerType, filter.IdTradeClass, filter.CustomerFirstName, filter.CustomerLastName, filter.ShipFirstName,
                filter.ShipLastName, filter.ShippingIdConfirmation, filter.IdOrder, filter.PoNumber, filter.CustomerCompany,
                filter.Paging?.PageIndex, filter.Paging?.PageItemCount).ToListAsync();
            return toReturn.Select(p => p.Id).ToList();
        }

        public async Task<int> GetCountOrderIdsForWholesaleDropShipReportAsync(WholesaleDropShipReportFilter filter)
        {
            if (string.IsNullOrEmpty(filter.CustomerCompany))
                filter.CustomerCompany = null;
            if (string.IsNullOrEmpty(filter.CustomerFirstName))
                filter.CustomerFirstName = null;
            if (string.IsNullOrEmpty(filter.CustomerLastName))
                filter.CustomerLastName = null;
            if (string.IsNullOrEmpty(filter.ShipFirstName))
                filter.ShipFirstName = null;
            if (string.IsNullOrEmpty(filter.ShipLastName))
                filter.ShipLastName = null;
            if (string.IsNullOrEmpty(filter.ShippingIdConfirmation))
                filter.ShippingIdConfirmation = null;

            var toReturn = await _context.Value.Set<CountModel>().FromSql
                ("[dbo].[SPGetOrderIdsForWholesaleDropShipReport] @from={0}, @to={1}, @shipfrom={2}, @shipto={3}," +
                 " @idcustomertype={4}, @idtradeclass={5}, @customerfirstname={6}, @customerlastname={7}, @shipfirstname={8}," +
                 " @shiplastname={9}, @shipidconfirm={10}, @idorder={11}, @ponumber={12}, @customercompany={13}, @getcount={14}",
                    filter.From, filter.To, filter.ShipFrom, filter.ShipTo,
                    filter.IdCustomerType, filter.IdTradeClass, filter.CustomerFirstName, filter.CustomerLastName, filter.ShipFirstName,
                    filter.ShipLastName, filter.ShippingIdConfirmation, filter.IdOrder, filter.PoNumber, filter.CustomerCompany, true)
                .FirstOrDefaultAsync();
            return toReturn?.Count ?? 0;
        }

        public async Task<ICollection<WholesaleDropShipReportSkuRawItem>> GetWholesaleDropShipReportSkusSummaryAsync(
            WholesaleDropShipReportFilter filter)
        {
            if (string.IsNullOrEmpty(filter.CustomerCompany))
                filter.CustomerCompany = null;
            if (string.IsNullOrEmpty(filter.CustomerFirstName))
                filter.CustomerFirstName = null;
            if (string.IsNullOrEmpty(filter.CustomerLastName))
                filter.CustomerLastName = null;
            if (string.IsNullOrEmpty(filter.ShipFirstName))
                filter.ShipFirstName = null;
            if (string.IsNullOrEmpty(filter.ShipLastName))
                filter.ShipLastName = null;
            if (string.IsNullOrEmpty(filter.ShippingIdConfirmation))
                filter.ShippingIdConfirmation = null;

            var toReturn = await _context.Value.Set<WholesaleDropShipReportSkuRawItem>().FromSql
            ("[dbo].[SPGetWholesaleDropShipReportSkusSummary] @from={0}, @to={1}, @shipfrom={2}, @shipto={3}," +
             " @idcustomertype={4}, @idtradeclass={5}, @customerfirstname={6}, @customerlastname={7}, @shipfirstname={8}," +
             " @shiplastname={9}, @shipidconfirm={10}, @idorder={11}, @ponumber={12}, @customercompany={13}",
                filter.From, filter.To, filter.ShipFrom, filter.ShipTo,
                filter.IdCustomerType, filter.IdTradeClass, filter.CustomerFirstName, filter.CustomerLastName, filter.ShipFirstName,
                filter.ShipLastName, filter.ShippingIdConfirmation, filter.IdOrder, filter.PoNumber, filter.CustomerCompany).ToListAsync();
            return toReturn;
        }

        public async Task<ICollection<TransactionAndRefundRawItem>> GetTransactionAndRefundReportItemsAsync(
            TransactionAndRefundReportFilter filter)
        {
            if (string.IsNullOrEmpty(filter.CustomerFirstName))
                filter.CustomerFirstName = null;
            if (string.IsNullOrEmpty(filter.CustomerLastName))
                filter.CustomerLastName = null;

            var toReturn = await _context.Value.Set<TransactionAndRefundRawItem>().FromSql
                ("[dbo].[SPGetTransactionAndRefundReport] @from={0}, @to={1}," +
                 " @idcustomertype={2}, @idservicecode={3}, @idcustomer={4}, @customerfirstname={5}, @customerlastname={6}," +
                 " @idorder={7}, @idorderstatus={8}, @idordertype={9}, @pageindex={10}, @pagesize={11}",
                    filter.From, filter.To,
                    filter.IdCustomerType, filter.IdServiceCode, filter.IdCustomer, filter.CustomerFirstName, filter.CustomerLastName,
                    filter.IdOrder, filter.IdOrderStatus, filter.IdOrderType, filter.Paging?.PageIndex, filter.Paging?.PageItemCount)
                .ToListAsync();
            return toReturn;
        }

        public async Task<ICollection<OrdersSummarySalesOrderTypeStatisticItem>> GetOrdersSummarySalesOrderTypeStatisticItemsAsync(
            OrdersSummarySalesReportFilter filter)
        {
            if (string.IsNullOrEmpty(filter.CustomerSourceDetails))
                filter.CustomerSourceDetails = null;
            if (string.IsNullOrEmpty(filter.KeyCode))
                filter.KeyCode = null;
            if (string.IsNullOrEmpty(filter.DiscountCode))
                filter.DiscountCode = null;

            var toReturn = await _context.Value.Set<OrdersSummarySalesOrderTypeStatisticItem>().FromSql
            ("[dbo].[SPGetOrderStatisticByTypeForOrdersSummarySalesReport] @from={0}, @to={1}," +
             " @idcustomersource={2}, @customersourcedetails={3}, @fromcount={4}, @tocount={5}, @keycode={6}," +
             " @idcustomer={7}, @firstorderfrom={8}, @firstorderto={9}, @idcustomertype={10}, @discountcode={11}, @isaffiliate={12}," +
             " @shipfrom={13}, @shipto={14}",
                filter.From, filter.To,
                filter.IdCustomerSource, filter.CustomerSourceDetails, filter.FromCount, filter.ToCount, filter.KeyCode,
                filter.IdCustomer, filter.FirstOrderFrom, filter.FirstOrderTo, filter.IdCustomerType, filter.DiscountCode,
                filter.IsAffiliate,
                filter.ShipFrom, filter.ShipTo).ToListAsync();
            return toReturn;
        }

        public async Task<ICollection<OrdersSummarySalesOrderItem>> GetOrdersSummarySalesOrderItemsAsync(
            OrdersSummarySalesReportFilter filter)
        {
            if (string.IsNullOrEmpty(filter.CustomerSourceDetails))
                filter.CustomerSourceDetails = null;
            if (string.IsNullOrEmpty(filter.KeyCode))
                filter.KeyCode = null;
            if (string.IsNullOrEmpty(filter.DiscountCode))
                filter.DiscountCode = null;

            var toReturn = await _context.Value.Set<OrdersSummarySalesOrderItem>().FromSql
            ("[dbo].[SPGetOrdersForOrdersSummarySalesReport] @from={0}, @to={1}," +
             " @idcustomersource={2}, @customersourcedetails={3}, @fromcount={4}, @tocount={5}, @keycode={6}," +
             " @idcustomer={7}, @firstorderfrom={8}, @firstorderto={9}, @idcustomertype={10}, @discountcode={11}, @isaffiliate={12}," +
             " @shipfrom={13}, @shipto={14}," +
             " @pageindex={15}, @pagesize={16}",
                filter.From, filter.To,
                filter.IdCustomerSource, filter.CustomerSourceDetails, filter.FromCount, filter.ToCount, filter.KeyCode,
                filter.IdCustomer, filter.FirstOrderFrom, filter.FirstOrderTo, filter.IdCustomerType, filter.DiscountCode,
                filter.IsAffiliate,
                filter.ShipFrom, filter.ShipTo,
                filter.Paging?.PageIndex, filter.Paging?.PageItemCount).ToListAsync();
            return toReturn;
        }

        public async Task<ICollection<CustomerOrdersTotal>> GetCustomersStandardOrderTotalsAsync(IList<int> ids, DateTime from, DateTime to)
        {
            string sIds = string.Empty;
            if (ids != null)
            {
                ids = ids.Distinct().ToList();
                for (int i = 0; i < ids.Count; i++)
                {
                    sIds += ids[i];
                    if (i != ids.Count - 1)
                    {
                        sIds += ",";
                    }
                }
            }

            var toReturn = await _context.Value.Set<CustomerOrdersTotal>().FromSql
            ("[dbo].[SPGetCustomersStandardOrderTotals] @from={0}, @to={1}, @customerids={2}",
                from, to, sIds).ToListAsync();
            return toReturn;
        }

        public async Task<ICollection<CustomerLastOrder>> GetCustomersStandardOrdersLastAsync(IList<int> ids)
        {
            string sIds = string.Empty;
            if (ids != null)
            {
                ids = ids.Distinct().ToList();
                for (int i = 0; i < ids.Count; i++)
                {
                    sIds += ids[i];
                    if (i != ids.Count - 1)
                    {
                        sIds += ",";
                    }
                }
            }

            var toReturn = await _context.Value.Set<CustomerLastOrder>().FromSql
                ("[dbo].[SPGetCustomersStandardOrdersLast] @customerids={0}", sIds).ToListAsync();
            return toReturn;
        }


        public async Task<ICollection<SkuBreakDownReportRawItem>> GetSkuBreakDownReportRawItemsAsync(SkuBreakDownReportFilter filter)
        {
            var toReturn = await _context.Value.Set<SkuBreakDownReportRawItem>().FromSql
            ("[dbo].[SPGetSkuBreakDownReport] @from={0}, @to={1}",
                filter.From, filter.To).ToListAsync();
            return toReturn;
        }

        public async Task<ICollection<SkuPOrderTypeBreakDownReportRawItem>> GetSkuPOrderTypeBreakDownReportRawItemsAsync(
            SkuPOrderTypeBreakDownReportFilter filter)
        {
            if (string.IsNullOrEmpty(filter.Code))
                filter.Code = null;

            var toReturn = await _context.Value.Set<SkuPOrderTypeBreakDownReportRawItem>().FromSql
            ("[dbo].[SPGetSkuPOrderTypeBreakDownReport] @from={0}, @to={1}, @code={2}",
                filter.From, filter.To, filter.Code).ToListAsync();
            return toReturn;
        }

        public async Task<ICollection<SkuPOrderTypeFutureBreakDownReportRawItem>> GetSkuPOrderTypeFutureBreakDownReportRawItemsAsync(
            SkuPOrderTypeBreakDownReportFilter filter)
        {
            if (string.IsNullOrEmpty(filter.Code))
                filter.Code = null;

            var toReturn = await _context.Value.Set<SkuPOrderTypeFutureBreakDownReportRawItem>().FromSql
            ("[dbo].[SPGetSkuPOrderTypeFutureBreakDownReport] @from={0}, @to={1}, @code={2}",
                filter.From, filter.To, filter.Code).ToListAsync();
            return toReturn;
        }

        public async Task<ICollection<MailingReportItem>> GetMailingReportRawItemsAsync(MailingReportFilter filter)
        {
            if (string.IsNullOrEmpty(filter.DiscountCodeFirst))
                filter.DiscountCodeFirst = null;
            if (string.IsNullOrEmpty(filter.KeyCodeFirst))
                filter.KeyCodeFirst = null;

            var toReturn = await _context.Value.Set<MailingReportItem>().FromSql
            ("[dbo].[SPGetMailingListReport] @from={0}, @to={1}," +
             " @idcustomertype={2}, @fromordercount={3}, @toordercount={4}, @fromfirst={5}, @tofirst={6}," +
             " @fromlast={7}, @tolast={8}, @lastfromtotal={9}, @lasttototal={10}, @dnm={11}, @dnr={12}," +
             " @idcustomerordersource={13}, @keycodefirst={14}, @discountcodefirst={15}," +
             " @pageindex={16}, @pagesize={17}",
                filter.From, filter.To,
                filter.CustomerIdObjectType, filter.FromOrderCount, filter.ToOrderCount, filter.FromFirst, filter.ToFirst,
                filter.FromLast, filter.ToLast, filter.LastFromTotal, filter.LastToTotal, filter.DNM, filter.DNR,
                filter.IdCustomerOrderSource, filter.KeyCodeFirst, filter.DiscountCodeFirst,
                filter.Paging?.PageIndex, filter.Paging?.PageItemCount).ToListAsync();
            return toReturn;
        }

        public async Task<ICollection<ShippedViaSummaryReportRawItem>> GetShippedViaSummaryReportRawItemsAsync(ShippedViaReportFilter filter)
        {
            var toReturn = await _context.Value.Set<ShippedViaSummaryReportRawItem>().FromSql
            ("[dbo].[SPGetShippedViaSummaryReport] @from={0}, @to={1}," +
             " @idstate={2}, @idservicecode={3}",
                filter.From, filter.To,
                filter.IdState, filter.IdServiceCode).ToListAsync();
            return toReturn;
        }

        public async Task<ICollection<ShippedViaReportRawOrderItem>> GetShippedViaItemsReportRawOrderItemsAsync(
            ShippedViaReportFilter filter)
        {
            var toReturn = await _context.Value.Set<ShippedViaReportRawOrderItem>().FromSql
            ("[dbo].[SPGetShippedViaItemsReport] @from={0}, @to={1}," +
             " @idstate={2}, @idservicecode={3}, @idwarehouse={4}, @carrier={5}, @idshipservice={6}," +
             " @pageindex={7}, @pagesize={8}",
                filter.From, filter.To,
                filter.IdState, filter.IdServiceCode, filter.IdWarehouse, filter.Carrier, filter.IdShipService,
                filter.Paging?.PageIndex, filter.Paging?.PageItemCount).ToListAsync();
            return toReturn;
        }

        public async Task<ICollection<ProductQualitySalesReportItem>> GetProductQualitySalesReportRawItemsAsync(
            ProductQualitySalesReportFilter filter)
        {
            var toReturn = await _context.Value.Set<ProductQualitySalesReportItem>().FromSql
            ("[dbo].[SPGetProductQualitySaleIssuesReport] @from={0}, @to={1}",
                filter.From, filter.To).ToListAsync();
            toReturn.ForEach(p =>
            {
                p.SalesPerIssue = Math.Round((decimal) p.Sales/p.Issues, 2);
            });
            return toReturn;
        }

        public async Task<ICollection<ProductQualitySkusReportItem>> GetProductQualitySkusReportRawItemsAsync(
            ProductQualitySkusReportFilter filter)
        {
            var toReturn = await _context.Value.Set<ProductQualitySkusReportItem>().FromSql
            ("[dbo].[SPGetProductQualitySkuIssuesReport] @from={0}, @to={1}, @idsku={2}",
                filter.From, filter.To, filter.IdSku).ToListAsync();
            return toReturn;
        }

        public async Task<KPIReportDBSaleRawItem> GetKPISalesRawItemAsync(DateTime from, DateTime to)
        {
            var toReturn = await _context.Value.Set<KPIReportDBSaleRawItem>().FromSql
            ("[dbo].[SPGetKPISales] @from={0}, @to={1}",
                from, to).FirstOrDefaultAsync();
            return toReturn;
        }

        public async Task<PagedList<ShortOrderItemModel>> GetShortOrdersAsync(OrderFilter filter)
        {
            filter.Id = filter.Id + "%";

            var data = await _context.Value.Set<ShortOrderItemModel>().FromSql
            ("[dbo].[SPGetShortOrders] @idcustomer={0}, @idfilter={1}," +
             " @idobjecttype={2}, @pageindex={3}, @pagesize={4}, @onlytop={5}",
                filter.IdCustomer, filter.Id,
                filter.OrderType, filter.Paging?.PageIndex, filter.Paging?.PageItemCount, filter.SelectOnlyTop).ToListAsync();
            var toReturn = new PagedList<ShortOrderItemModel>();
            toReturn.Count = data.FirstOrDefault()?.TotalCount ?? 0;
            toReturn.Items = data;
            return toReturn;
        }

        public async Task<ICollection<AAFESReportItem>> GetAAFESReportItemsAsync(AAFESReportFilter filter)
        {
            var toReturn = await _context.Value.Set<AAFESReportItem>().FromSql
            ("[dbo].[SPGetAAFESReport] @from={0}, @to={1}, @shipfrom={2}, @shipto={3}",
                filter.From, filter.To, filter.ShipFrom, filter.ShipTo).ToListAsync();
            return toReturn;
        }

        public async Task<PagedList<CustomerSkuUsageReportRawItem>> GetCustomerSkuUsageReportItemsAsync(CustomerSkuUsageReportFilter filter)
        {
            string sSkuIds = null;
            if (filter.SkuIds != null && filter.SkuIds.Count > 0)
            {
                sSkuIds = string.Empty;
                for (int i = 0; i < filter.SkuIds.Count; i++)
                {
                    sSkuIds += filter.SkuIds[i];
                    if (i != filter.SkuIds.Count - 1)
                    {
                        sSkuIds += ",";
                    }
                }
            }

            var data =
                await
                    _context.Value.Set<CustomerSkuUsageReportRawItem>()
                        .FromSql("[dbo].[SPGetCustomerSkuUsageReport] @from={0}, @to={1}, @skus={2}, @idcategory={3}," +
                                 "@idcustomertype={4}, @pageindex={5}, @pagesize={6}",
                            filter.From, filter.To, sSkuIds, filter.IdCategory,
                            filter.IdCustomerType, filter.Paging?.PageIndex, filter.Paging?.PageItemCount).ToListAsync();
            var toReturn = new PagedList<CustomerSkuUsageReportRawItem>();
            toReturn.Count = data.FirstOrDefault()?.TotalCount ?? 0;
            toReturn.Items = data;
            return toReturn;
        }

        public async Task<PagedList<OrderDiscountReportItem>> GetOrderDiscountReportItemsAsync(OrderDiscountReportFilter filter)
        {
            if (string.IsNullOrEmpty(filter.Discount))
                filter.Discount = null;

            var data = await _context.Value.Set<OrderDiscountReportItem>().FromSql
            ("[dbo].[SPGetOrderDiscountReport] @from={0}, @to={1}, @discount={2}, @pageindex={3}, @pagesize={4}",
                filter.From, filter.To, filter.Discount,
                filter.Paging?.PageIndex, filter.Paging?.PageItemCount).ToListAsync();

            var toReturn = new PagedList<OrderDiscountReportItem>();
            toReturn.Count = data.FirstOrDefault()?.TotalCount ?? 0;
            toReturn.Items = data;

            return toReturn;
        }

        public async Task<ICollection<SkuAverageDailySalesReportRawItem>> GetSkuAverageDailySalesReportRawItemsAsync(
            SkuAverageDailySalesReportFilter filter)
        {
            string sSkuIds = null;
            if (filter.SkuIds != null && filter.SkuIds.Count > 0)
            {
                sSkuIds = string.Empty;
                for (int i = 0; i < filter.SkuIds.Count; i++)
                {
                    sSkuIds += filter.SkuIds[i];
                    if (i != filter.SkuIds.Count - 1)
                    {
                        sSkuIds += ",";
                    }
                }
            }

            if (sSkuIds == null && !string.IsNullOrEmpty(filter.ProductName))
            {
                sSkuIds = "";
            }

            var toReturn = await _context.Value.Set<SkuAverageDailySalesReportRawItem>().FromSql
            ("[dbo].[SPGetSkuAverageDailySalesReport] @from={0}, @to={1}, @idcustomertype={2}, @skus={3}, @mode={4}",
                filter.From, filter.To, filter.IdCustomerType, sSkuIds, (int) filter.Mode).ToListAsync();

            return toReturn;
        }

        public async Task<ICollection<int>> GetCustomersForReviewAsync(IList<int> ids, DateTime from, DateTime to)
        {
            string sIds = string.Empty;
            if (ids != null)
            {
                ids = ids.Distinct().ToList();
                for (int i = 0; i < ids.Count; i++)
                {
                    sIds += ids[i];
                    if (i != ids.Count - 1)
                    {
                        sIds += ",";
                    }
                }
            }

            var toReturn = await _context.Value.Set<IdModel>().FromSql
            ("[dbo].[SPGetCustomersForReview] @from={0}, @to={1}, @customerids={2}",
                from, to, sIds).ToListAsync();
            return toReturn.Select(p=>p.Id).ToList();
        }

        public async Task<IList<OrderAbuseReportRawItem>> GetOrderAbuseReportRawItemsAsync(OrdersAbuseReportFilter filter)
        {
            var toReturn = await _context.Value.Set<OrderAbuseReportRawItem>().FromSql
            ("[dbo].[SPGetOrdersAbuseReport] @from={0}, @to={1}, @reships={2}, @refunds={3}, @reshipsorrefunds={4}," +
                "@idservicecode={5}, @idcustomer={6}",
                filter.From, filter.To, filter.Reships, filter.Refunds, filter.ReshipsOrRefunds,
                filter.IdServiceCode, filter.IdCustomer).ToListAsync();
            return toReturn;
        }

        public async Task<IList<CustomerOrderAbuseReportRawItem>> GetCustomerOrdersAbuseRawItemsAsync(OrdersAbuseReportFilter filter)
        {
            var toReturn = await _context.Value.Set<CustomerOrderAbuseReportRawItem>().FromSql
            ("[dbo].[SPGetCustomersAbuseReport] @from={0}, @to={1}, @reships={2}, @refunds={3}, @reshipsorrefunds={4}," +
                "@idservicecode={5}, @idcustomer={6}",
                filter.From, filter.To, filter.Reships, filter.Refunds, filter.ReshipsOrRefunds,
                filter.IdServiceCode, filter.IdCustomer).ToListAsync();
            return toReturn;
        }

        public void Dispose()
        {
            if (_context.IsValueCreated)
            {
                _context.Value.Dispose();
            }
        }
    }
}