using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using VitalChoice.Data.Context;
using VitalChoice.Data.Repositories.Specifics;
using VitalChoice.Ecommerce.Domain;
using VitalChoice.Ecommerce.Domain.Transfer;
using VitalChoice.Infrastructure.Context;
using VitalChoice.Infrastructure.Domain.Entities;
using VitalChoice.Infrastructure.Domain.Entities.Customers;
using VitalChoice.Infrastructure.Domain.Entities.InventorySkus;
using VitalChoice.Infrastructure.Domain.Entities.Products;
using VitalChoice.Infrastructure.Domain.Entities.Reports;
using VitalChoice.Infrastructure.Domain.Transfer.InventorySkus;
using VitalChoice.Infrastructure.Domain.Transfer.Orders;
using VitalChoice.Infrastructure.Domain.Transfer.Products;
using VitalChoice.Infrastructure.Domain.Transfer.Reports;

namespace VitalChoice.Business.Repositories
{
    public class SpEcommerceRepository
    {
        private readonly EcommerceContext _context;

        public SpEcommerceRepository(EcommerceContext context)
        {
            _context = context;
        }

        public async Task<ICollection<ProductCategoryStatisticItem>> GetProductCategoryStatisticAsync(DateTime from, DateTime to)
        {
            var toReturn = await _context.Set<ProductCategoryStatisticItem>().FromSql("[dbo].[SPGetProductCategoryStatistic] @from={0}, @to={1}", from, to).ToListAsync();
            return toReturn;
        }

        public async Task<ICollection<SkusInProductCategoryStatisticItem>> GetSkusInProductCategoryStatisticAsync(DateTime from,DateTime to, int idCategory)
        {
            var toReturn = await _context.Set<SkusInProductCategoryStatisticItem>().FromSql("[dbo].[SPGetSkusInProductCategoryStatistic] @from={0}, @to={1}, @idcategory={2}", from,to, idCategory).ToListAsync();
            return toReturn;
        }

        public async Task<ICollection<OrdersRegionStatisticItem>> GetOrdersRegionStatisticAsync(OrderRegionFilter filter)
        {
            var toReturn = await _context.Set<OrdersRegionStatisticItem>().FromSql("[dbo].[SPGetOrdersRegionStatistic] @from={0}, @to={1}, @IdCustomerType={2}, @IdOrderType={3}",
                filter.From, filter.To, filter.IdCustomerType, filter.IdOrderType).ToListAsync();
            return toReturn;
        }

        public async Task<ICollection<OrdersZipStatisticItem>> GetOrdersZipStatisticAsync(OrderRegionFilter filter)
        {
            var toReturn = await _context.Set<OrdersZipStatisticItem>().FromSql("[dbo].[SPGetOrdersZipStatistic] @from={0}, @to={1}, @IdCustomerType={2}, @IdOrderType={3}",
                filter.From, filter.To, filter.IdCustomerType, filter.IdOrderType).ToListAsync();
            return toReturn;
        }

        public async Task<ICollection<InventorySkuUsageRawReportItem>> GetInventorySkusUsageReportAsync(InventorySkuUsageReportFilter filter)
        {
            string sSkuIds = null;
            if (filter.SkuIds != null && filter.SkuIds.Count>0)
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

            var toReturn = await _context.Set<InventorySkuUsageRawReportItem>().FromSql("[dbo].[SPGetInventorySkusUsageReport] @from={0}, @to={1}, @skus={2}, @invskus={3}",
                filter.From, filter.To, sSkuIds, sInvSkuIds).ToListAsync();
            return toReturn;
        }

        public async Task<ICollection<InventoriesSummaryUsageRawReportItem>> GetInventoriesSummaryUsageReportAsync(InventoriesSummaryUsageReportFilter filter)
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

            var toReturn = await _context.Set<InventoriesSummaryUsageRawReportItem>().FromSql("[dbo].[SPGetInventoriesSummaryUsageReport] @from={0}, @to={1}, @sku={2}, @invsku={3}, @assemble={4}, @idsinvcat={5}, @shipdate={6}",
                filter.From, filter.To, filter.Sku, filter.InvSku, filter.Assemble, sIdsInvCat, filter.ShipDate).ToListAsync();
            return toReturn;
        }

        public async Task<ICollection<int>> GetOrderIdsForWholesaleDropShipReportAsync(WholesaleDropShipReportFilter filter)
        {
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

            var toReturn = await _context.Set<IdModel>().FromSql
                ("[dbo].[SPGetOrderIdsForWholesaleDropShipReport] @from={0}, @to={1}, @shipfrom={2}, @shipto={3},"+
                " @idcustomertype={4}, @idtradeclass={5}, @customerfirstname={6}, @customerlastname={7}, @shipfirstname={8},"+
                " @shiplastname={9}, @shipidconfirm={10}, @idorder={11}, @ponumber={12}, @pageindex={13}, @pagesize={14}",
                filter.From, filter.To, filter.ShipFrom, filter.ShipTo,
                filter.IdCustomerType, filter.IdTradeClass, filter.CustomerFirstName, filter.CustomerLastName, filter.ShipFirstName,
                filter.ShipLastName, filter.ShippingIdConfirmation, filter.IdOrder, filter.PoNumber, filter.Paging?.PageIndex, filter.Paging?.PageItemCount).ToListAsync();
            return toReturn.Select(p=>p.Id).ToList();
        }

        public async Task<int> GetCountOrderIdsForWholesaleDropShipReportAsync(WholesaleDropShipReportFilter filter)
        {
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

            var toReturn = await _context.Set<CountModel>().FromSql
                ("[dbo].[SPGetOrderIdsForWholesaleDropShipReport] @from={0}, @to={1}, @shipfrom={2}, @shipto={3}," +
                " @idcustomertype={4}, @idtradeclass={5}, @customerfirstname={6}, @customerlastname={7}, @shipfirstname={8}," +
                " @shiplastname={9}, @shipidconfirm={10}, @idorder={11}, @ponumber={12}, @getcount={13}",
                filter.From, filter.To, filter.ShipFrom, filter.ShipTo,
                filter.IdCustomerType, filter.IdTradeClass, filter.CustomerFirstName, filter.CustomerLastName, filter.ShipFirstName,
                filter.ShipLastName, filter.ShippingIdConfirmation, filter.IdOrder, filter.PoNumber, true).FirstOrDefaultAsync();
            return toReturn?.Count ?? 0;
        }

        public async Task<ICollection<WholesaleDropShipReportSkuRawItem>> GetWholesaleDropShipReportSkusSummaryAsync(WholesaleDropShipReportFilter filter)
        {
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

            var toReturn = await _context.Set<WholesaleDropShipReportSkuRawItem>().FromSql
                ("[dbo].[SPGetWholesaleDropShipReportSkusSummary] @from={0}, @to={1}, @shipfrom={2}, @shipto={3}," +
                " @idcustomertype={4}, @idtradeclass={5}, @customerfirstname={6}, @customerlastname={7}, @shipfirstname={8}," +
                " @shiplastname={9}, @shipidconfirm={10}, @idorder={11}, @ponumber={12}",
                filter.From, filter.To, filter.ShipFrom, filter.ShipTo,
                filter.IdCustomerType, filter.IdTradeClass, filter.CustomerFirstName, filter.CustomerLastName, filter.ShipFirstName,
                filter.ShipLastName, filter.ShippingIdConfirmation, filter.IdOrder, filter.PoNumber).ToListAsync();
            return toReturn;
        }

        public async Task<ICollection<TransactionAndRefundRawItem>> GetTransactionAndRefundReportItemsAsync(TransactionAndRefundReportFilter filter)
        {
            if (string.IsNullOrEmpty(filter.CustomerFirstName))
                filter.CustomerFirstName = null;
            if (string.IsNullOrEmpty(filter.CustomerLastName))
                filter.CustomerLastName = null;

            var toReturn = await _context.Set<TransactionAndRefundRawItem>().FromSql
                ("[dbo].[SPGetTransactionAndRefundReport] @from={0}, @to={1}," +
                " @idcustomertype={2}, @idservicecode={3}, @idcustomer={4}, @customerfirstname={5}, @customerlastname={6}," +
                " @idorder={7}, @idorderstatus={8}, @idordertype={9}, @pageindex={10}, @pagesize={11}",
                filter.From, filter.To, 
                filter.IdCustomerType, filter.IdServiceCode, filter.IdCustomer, filter.CustomerFirstName, filter.CustomerLastName,
                filter.IdOrder, filter.IdOrderStatus, filter.IdOrderType, filter.Paging?.PageIndex, filter.Paging?.PageItemCount).ToListAsync();
            return toReturn;
        }

        public async Task<ICollection<OrdersSummarySalesOrderTypeStatisticItem>> GetOrdersSummarySalesOrderTypeStatisticItemsAsync(OrdersSummarySalesReportFilter filter)
        {
            if (string.IsNullOrEmpty(filter.CustomerSourceDetails))
                filter.CustomerSourceDetails = null;
            if (string.IsNullOrEmpty(filter.KeyCode))
                filter.KeyCode = null;
            if (string.IsNullOrEmpty(filter.DiscountCode))
                filter.DiscountCode = null;

            var toReturn = await _context.Set<OrdersSummarySalesOrderTypeStatisticItem>().FromSql
                ("[dbo].[SPGetOrderStatisticByTypeForOrdersSummarySalesReport] @from={0}, @to={1}," +
                " @idcustomersource={2}, @customersourcedetails={3}, @fromcount={4}, @tocount={5}, @keycode={6}," +
                " @idcustomer={7}, @firstorderfrom={8}, @firstorderto={9}, @idcustomertype={10}, @discountcode={11}, @isaffiliate={12}," +
                " @shipfrom={13}, @shipto={14}",
                filter.From, filter.To,
                filter.IdCustomerSource, filter.CustomerSourceDetails, filter.FromCount, filter.ToCount, filter.KeyCode,
                filter.IdCustomer, filter.FirstOrderFrom, filter.FirstOrderTo, filter.IdCustomerType, filter.DiscountCode, filter.IsAffiliate,
                filter.ShipFrom, filter.ShipTo).ToListAsync();
            return toReturn;
        }

        public async Task<ICollection<OrdersSummarySalesOrderItem>> GetOrdersSummarySalesOrderItemsAsync(OrdersSummarySalesReportFilter filter)
        {
            if (string.IsNullOrEmpty(filter.CustomerSourceDetails))
                filter.CustomerSourceDetails = null;
            if (string.IsNullOrEmpty(filter.KeyCode))
                filter.KeyCode = null;
            if (string.IsNullOrEmpty(filter.DiscountCode))
                filter.DiscountCode = null;

            var toReturn = await _context.Set<OrdersSummarySalesOrderItem>().FromSql
                ("[dbo].[SPGetOrdersForOrdersSummarySalesReport] @from={0}, @to={1}," +
                " @idcustomersource={2}, @customersourcedetails={3}, @fromcount={4}, @tocount={5}, @keycode={6}," +
                " @idcustomer={7}, @firstorderfrom={8}, @firstorderto={9}, @idcustomertype={10}, @discountcode={11}, @isaffiliate={12}," +
                " @shipfrom={13}, @shipto={14}," +
                " @pageindex={15}, @pagesize={16}",
                filter.From, filter.To,
                filter.IdCustomerSource, filter.CustomerSourceDetails, filter.FromCount, filter.ToCount, filter.KeyCode,
                filter.IdCustomer, filter.FirstOrderFrom, filter.FirstOrderTo, filter.IdCustomerType, filter.DiscountCode, filter.IsAffiliate,
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

            var toReturn = await _context.Set<CustomerOrdersTotal>().FromSql
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

            var toReturn = await _context.Set<CustomerLastOrder>().FromSql
                ("[dbo].[SPGetCustomersStandardOrdersLast] @customerids={0}",sIds).ToListAsync();
            return toReturn;
        }


        public async Task<ICollection<SkuBreakDownReportRawItem>> GetSkuBreakDownReportRawItemsAsync(SkuBreakDownReportFilter filter)
        {
            var toReturn = await _context.Set<SkuBreakDownReportRawItem>().FromSql
                ("[dbo].[SPGetSkuBreakDownReport] @from={0}, @to={1}",
                filter.From, filter.To).ToListAsync();
            return toReturn;
        }

        public async Task<ICollection<SkuPOrderTypeBreakDownReportRawItem>> GetSkuPOrderTypeBreakDownReportRawItemsAsync(SkuPOrderTypeBreakDownReportFilter filter)
        {
            var toReturn = await _context.Set<SkuPOrderTypeBreakDownReportRawItem>().FromSql
                ("[dbo].[SPGetSkuPOrderTypeBreakDownReport] @from={0}, @to={1}",
                filter.From, filter.To).ToListAsync();
            return toReturn;
        }

        public async Task<ICollection<SkuPOrderTypeFutureBreakDownReportRawItem>> GetSkuPOrderTypeFutureBreakDownReportRawItemsAsync(SkuPOrderTypeBreakDownReportFilter filter)
        {
            var toReturn = await _context.Set<SkuPOrderTypeFutureBreakDownReportRawItem>().FromSql
                ("[dbo].[SPGetSkuPOrderTypeFutureBreakDownReport] @from={0}, @to={1}",
                filter.From, filter.To).ToListAsync();
            return toReturn;
        }

        public async Task<ICollection<MailingReportItem>> GetMailingReportRawItemsAsync(MailingReportFilter filter)
        {
            if (string.IsNullOrEmpty(filter.DiscountCodeFirst))
                filter.DiscountCodeFirst = null;
            if (string.IsNullOrEmpty(filter.KeyCodeFirst))
                filter.KeyCodeFirst = null;

            var toReturn = await _context.Set<MailingReportItem>().FromSql
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
            var toReturn = await _context.Set<ShippedViaSummaryReportRawItem>().FromSql
                ("[dbo].[SPGetShippedViaSummaryReport] @from={0}, @to={1}," +
                " @idstate={2}, @idservicecode={3}",
                filter.From, filter.To,
                filter.IdState, filter.IdServiceCode).ToListAsync();
            return toReturn;
        }

        public async Task<ICollection<ShippedViaReportRawOrderItem>> GetShippedViaItemsReportRawOrderItemsAsync(ShippedViaReportFilter filter)
        {
            var toReturn = await _context.Set<ShippedViaReportRawOrderItem>().FromSql
                ("[dbo].[SPGetShippedViaItemsReport] @from={0}, @to={1}," +
                " @idstate={2}, @idservicecode={3}, @idwarehouse={4}, @carrier={5}, @idshipservice={6}," +
                " @pageindex={7}, @pagesize={8}",
                filter.From, filter.To,
                filter.IdState, filter.IdServiceCode, filter.IdWarehouse, filter.Carrier, filter.IdShipService,
                filter.Paging?.PageIndex, filter.Paging?.PageItemCount).ToListAsync();
            return toReturn;
        }

        public async Task<ICollection<ProductQualitySalesReportItem>> GetProductQualitySalesReportRawItemsAsync(ProductQualitySalesReportFilter filter)
        {
            var toReturn = await _context.Set<ProductQualitySalesReportItem>().FromSql
                ("[dbo].[SPGetProductQualitySaleIssuesReport] @from={0}, @to={1}",
                filter.From, filter.To).ToListAsync();
            toReturn.ForEach(p =>
            {
                p.SalesPerIssue = Math.Round((decimal)p.Sales/p.Issues, 2);
            });
            return toReturn;
        }

        public async Task<ICollection<ProductQualitySkusReportItem>> GetProductQualitySkusReportRawItemsAsync(ProductQualitySkusReportFilter filter)
        {
            var toReturn = await _context.Set<ProductQualitySkusReportItem>().FromSql
                ("[dbo].[SPGetProductQualitySkuIssuesReport] @from={0}, @to={1}, @idsku={2}",
                filter.From, filter.To, filter.IdSku).ToListAsync();
            return toReturn;
        }
    }
}
