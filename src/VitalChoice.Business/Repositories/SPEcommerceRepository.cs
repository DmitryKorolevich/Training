﻿using Microsoft.Data.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VitalChoice.Data.Context;
using VitalChoice.Data.Repositories.Specifics;
using VitalChoice.Data.Extensions;
using System.Linq.Expressions;
using VitalChoice.Ecommerce.Domain.Entities;
using VitalChoice.Ecommerce.Domain.Entities.Affiliates;
using VitalChoice.Ecommerce.Domain.Entities.Orders;
using VitalChoice.Infrastructure.Domain.Entities;
using VitalChoice.Infrastructure.Domain.Transfer.Affiliates;
using VitalChoice.Ecommerce.Domain;
using VitalChoice.Infrastructure.Domain.Entities.Customers;
using VitalChoice.Infrastructure.Domain.Entities.InventorySkus;
using VitalChoice.Infrastructure.Domain.Entities.Orders;
using VitalChoice.Infrastructure.Domain.Entities.Products;
using VitalChoice.Infrastructure.Domain.Entities.Reports;
using VitalChoice.Infrastructure.Domain.Transfer.InventorySkus;
using VitalChoice.Infrastructure.Domain.Transfer.Orders;
using VitalChoice.Infrastructure.Domain.Transfer.Reports;

namespace VitalChoice.Data.Repositories.Customs
{
    public class SPEcommerceRepository : EcommerceRepositoryAsync<Entity>
    {
        public SPEcommerceRepository(
            IDataContextAsync context) : base(context)
		{
        }

        public async Task<ICollection<ProductCategoryStatisticItem>> GetProductCategoryStatisticAsync(DateTime from, DateTime to)
        {
            var toReturn = await Context.Set<ProductCategoryStatisticItem>().FromSql("[dbo].[SPGetProductCategoryStatistic] @from={0}, @to={1}", from, to).ToListAsync();
            return toReturn;
        }

        public async Task<ICollection<SkusInProductCategoryStatisticItem>> GetSkusInProductCategoryStatisticAsync(DateTime from,DateTime to, int idCategory)
        {
            var toReturn = await Context.Set<SkusInProductCategoryStatisticItem>().FromSql("[dbo].[SPGetSkusInProductCategoryStatistic] @from={0}, @to={1}, @idcategory={2}", from,to, idCategory).ToListAsync();
            return toReturn;
        }

        public async Task<ICollection<OrdersRegionStatisticItem>> GetOrdersRegionStatisticAsync(OrderRegionFilter filter)
        {
            var toReturn = await Context.Set<OrdersRegionStatisticItem>().FromSql("[dbo].[SPGetOrdersRegionStatistic] @from={0}, @to={1}, @IdCustomerType={2}, @IdOrderType={3}",
                filter.From, filter.To, filter.IdCustomerType, filter.IdOrderType).ToListAsync();
            return toReturn;
        }

        public async Task<ICollection<OrdersZipStatisticItem>> GetOrdersZipStatisticAsync(OrderRegionFilter filter)
        {
            var toReturn = await Context.Set<OrdersZipStatisticItem>().FromSql("[dbo].[SPGetOrdersZipStatistic] @from={0}, @to={1}, @IdCustomerType={2}, @IdOrderType={3}",
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

            var toReturn = await Context.Set<InventorySkuUsageRawReportItem>().FromSql("[dbo].[SPGetInventorySkusUsageReport] @from={0}, @to={1}, @skus={2}, @invskus={3}",
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

            var toReturn = await Context.Set<InventoriesSummaryUsageRawReportItem>().FromSql("[dbo].[SPGetInventoriesSummaryUsageReport] @from={0}, @to={1}, @sku={2}, @invsku={3}, @assemble={4}, @idsinvcat={5}",
                filter.From, filter.To, filter.Sku, filter.InvSku, filter.Assemble, sIdsInvCat).ToListAsync();
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

            var toReturn = await Context.Set<IdModel>().FromSql
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

            var toReturn = await Context.Set<CountModel>().FromSql
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

            var toReturn = await Context.Set<WholesaleDropShipReportSkuRawItem>().FromSql
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

            var toReturn = await Context.Set<TransactionAndRefundRawItem>().FromSql
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

            var toReturn = await Context.Set<OrdersSummarySalesOrderTypeStatisticItem>().FromSql
                ("[dbo].[SPGetOrderStatisticByTypeForOrdersSummarySalesReport] @from={0}, @to={1}," +
                " @idcustomersource={2}, @customersourcedetails={3}, @fromcount={4}, @tocount={5}, @keycode={6}," +
                " @idcustomer={7}, @firstorderfrom={8}, @firstorderto={9}, @idcustomertype={10}, @discountcode={11}, @isaffiliate={12}",
                filter.From, filter.To,
                filter.IdCustomerSource, filter.CustomerSourceDetails, filter.FromCount, filter.ToCount, filter.KeyCode,
                filter.IdCustomer, filter.FirstOrderFrom, filter.FirstOrderTo, filter.IdCustomerType, filter.DiscountCode, filter.IsAffiliate).ToListAsync();
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

            var toReturn = await Context.Set<OrdersSummarySalesOrderItem>().FromSql
                ("[dbo].[SPGetOrdersForOrdersSummarySalesReport] @from={0}, @to={1}," +
                " @idcustomersource={2}, @customersourcedetails={3}, @fromcount={4}, @tocount={5}, @keycode={6}," +
                " @idcustomer={7}, @firstorderfrom={8}, @firstorderto={9}, @idcustomertype={10}, @discountcode={11}, @isaffiliate={12}," +
                " @pageindex={13}, @pagesize={14}",
                filter.From, filter.To,
                filter.IdCustomerSource, filter.CustomerSourceDetails, filter.FromCount, filter.ToCount, filter.KeyCode,
                filter.IdCustomer, filter.FirstOrderFrom, filter.FirstOrderTo, filter.IdCustomerType, filter.DiscountCode, filter.IsAffiliate,
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

            var toReturn = await Context.Set<CustomerOrdersTotal>().FromSql
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

            var toReturn = await Context.Set<CustomerLastOrder>().FromSql
                ("[dbo].[SPGetCustomersStandardOrdersLast] @customerids={0}",sIds).ToListAsync();
            return toReturn;
        }
    }
}
