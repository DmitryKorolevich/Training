using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using VitalChoice.Business.Queries.Orders;
using VitalChoice.Business.Queries.Users;
using VitalChoice.Data.Repositories;
using VitalChoice.Data.Repositories.Specifics;
using VitalChoice.Data.Transaction;
using VitalChoice.Ecommerce.Domain.Entities;
using VitalChoice.Ecommerce.Domain.Entities.Orders;
using VitalChoice.Ecommerce.Domain.Exceptions;
using VitalChoice.Ecommerce.Domain.Transfer;
using VitalChoice.Infrastructure.Context;
using VitalChoice.Infrastructure.Domain.Constants;
using VitalChoice.Infrastructure.Domain.Entities.Users;
using VitalChoice.Infrastructure.Domain.Transfer;
using VitalChoice.Infrastructure.Domain.Transfer.Orders;
using VitalChoice.Interfaces.Services;
using VitalChoice.Interfaces.Services.Orders;
using VitalChoice.Interfaces.Services.Settings;
using System.Collections.Generic;
using VitalChoice.Data.Helpers;
using VitalChoice.Ecommerce.Domain.Entities.Customers;
using VitalChoice.Ecommerce.Domain.Helpers;
using VitalChoice.Infrastructure.Domain.Dynamic;

namespace VitalChoice.Business.Services.Orders
{
    public class ServiceCodeService : IServiceCodeService
    {
        private readonly OrderService _orderService;
        private readonly OrderRefundService _orderRefundService;
        private readonly IEcommerceRepositoryAsync<Order> _orderRepository;
        private readonly ISettingService _settingService;
        private readonly ICountryService _countryService;
        private readonly ReferenceData _referenceData;
        private readonly ILogger _logger;

        public ServiceCodeService(
            OrderService orderService,
            OrderRefundService orderRefundService,
            IEcommerceRepositoryAsync<Order> orderRepository,
            ISettingService settingService,
            ICountryService countryService,
            ReferenceData referenceData,
            ILoggerFactory loggerProvider)
        {
            _orderService = orderService;
            _orderRefundService = orderRefundService;
            _orderRepository = orderRepository;
            _settingService = settingService;
            _countryService = countryService;
            _referenceData = referenceData;
            _logger = loggerProvider.CreateLogger<ServiceCodeService>();
        }

        public async Task<ServiceCodesReport> GetServiceCodesReportAsync(ServiceCodesReportFilter filter)
        {
            var toReturn = new ServiceCodesReport();
            toReturn.Items = new List<ServiceCodesReportItem>();
            OrderQuery conditions =
                new OrderQuery().NotDeleted().WithActualStatusOnly().WithFromDate(filter.From).WithToDate(filter.To);
            toReturn.OrdersCount = await _orderRepository.Query(conditions).SelectCountAsync();
            toReturn.OrdersAmount = await _orderRepository.Query(conditions).SelectSumAsync(p => p.Total);

            var serviceCodesLookup =
                (await _settingService.GetLookupsAsync(new[] {SettingConstants.SERVICE_CODE_LOOKUP_NAME}))
                    .FirstOrDefault();
            if (serviceCodesLookup == null || serviceCodesLookup.LookupVariants == null)
            {
                return toReturn;
            }

            conditions = conditions.WithOrderType(OrderType.Reship);
            var reships = await _orderService.SelectAsync(conditions,
                x => x.Include(p => p.Customer).
                    Include(p => p.OptionValues).
                    Include(p => p.Skus).
                    ThenInclude(p => p.Sku));

            foreach (var reship in reships)
            {
                if (reship.SafeData.ServiceCode != null)
                {
                    var serviceCodesReportItem = toReturn.Items.FirstOrDefault(p => p.Id == reship.SafeData.ServiceCode);
                    if (serviceCodesReportItem == null)
                    {
                        serviceCodesReportItem = new ServiceCodesReportItem();
                        serviceCodesReportItem.Id = reship.SafeData.ServiceCode;
                        serviceCodesReportItem.Name =
                            serviceCodesLookup.LookupVariants.FirstOrDefault(p => p.Id == reship.SafeData.ServiceCode)?
                                .ValueVariant;
                        toReturn.Items.Add(serviceCodesReportItem);
                    }

                    serviceCodesReportItem.ReshipsCount++;
                    serviceCodesReportItem.TotalCount++;
                    if (reship.SafeData.ReturnAssociated == true)
                    {
                        serviceCodesReportItem.ReturnsCount++;
                        serviceCodesReportItem.ReturnsAmount += ProductConstants.DEFAULT_RETURN_AMOUNT;
                    }

                    var listAmount = reship.Customer.IdObjectType == (int) CustomerType.Retail
                        ? reship.Skus.Sum(p => p.Quantity*p.Sku.Price)
                        : reship.Skus.Sum(p => p.Quantity*p.Sku.WholesalePrice);
                    var reshipAmount = reship.Total - listAmount;
                    serviceCodesReportItem.ReshipsAmount += reshipAmount;
                    serviceCodesReportItem.TotalAmount += reshipAmount;
                }
            }

            conditions =
                new OrderQuery().NotDeleted().WithActualStatusOnly().WithFromDate(filter.From).WithToDate(filter.To);
            conditions = conditions.WithOrderType(OrderType.Refund);
            var refunds = await _orderService.SelectAsync(conditions, x => x);
            foreach (var refund in refunds)
            {
                if (refund.SafeData.ServiceCode != null)
                {
                    var serviceCodesReportItem = toReturn.Items.FirstOrDefault(p => p.Id == refund.SafeData.ServiceCode);
                    if (serviceCodesReportItem == null)
                    {
                        serviceCodesReportItem = new ServiceCodesReportItem();
                        serviceCodesReportItem.Id = refund.SafeData.ServiceCode;
                        serviceCodesReportItem.Name =
                            serviceCodesLookup.LookupVariants.FirstOrDefault(p => p.Id == refund.SafeData.ServiceCode)?
                                .ValueVariant;
                        toReturn.Items.Add(serviceCodesReportItem);
                    }

                    serviceCodesReportItem.RefundsCount++;
                    serviceCodesReportItem.TotalCount++;
                    if (refund.SafeData.ReturnAssociated == true)
                    {
                        serviceCodesReportItem.ReturnsCount++;
                        serviceCodesReportItem.ReturnsAmount += ProductConstants.DEFAULT_RETURN_AMOUNT;
                    }
                    serviceCodesReportItem.RefundsAmount -= refund.Total;
                    serviceCodesReportItem.TotalAmount -= refund.Total;
                }
            }

            //Calculate percents
            foreach (var serviceCodesReportItem in toReturn.Items)
            {
                serviceCodesReportItem.CountPercent = toReturn.OrdersCount != 0
                    ? ((decimal) serviceCodesReportItem.TotalCount)/toReturn.OrdersCount
                    : 0;
                serviceCodesReportItem.CountPercent = Math.Round(serviceCodesReportItem.CountPercent*100, 3);

                serviceCodesReportItem.AmountPercent = toReturn.OrdersAmount != 0
                    ? ((decimal) serviceCodesReportItem.TotalAmount)/toReturn.OrdersAmount
                    : 0;
                serviceCodesReportItem.AmountPercent = Math.Round(serviceCodesReportItem.AmountPercent*100, 3);
            }

            toReturn.Items =
                toReturn.Items.OrderBy(p => serviceCodesLookup.LookupVariants.FirstOrDefault(pp => p.Id == pp.Id)?.Order)
                    .ToList();
            //Totals
            var total = new ServiceCodesReportItem() {Name = "Total"};
            foreach (var serviceCodesReportItem in toReturn.Items)
            {
                total.ReshipsCount += serviceCodesReportItem.ReshipsCount;
                total.RefundsCount += serviceCodesReportItem.RefundsCount;
                total.ReturnsCount += serviceCodesReportItem.ReturnsCount;
                total.TotalCount += serviceCodesReportItem.TotalCount;
                total.CountPercent += serviceCodesReportItem.CountPercent;

                total.ReshipsAmount += serviceCodesReportItem.ReshipsAmount;
                total.RefundsAmount += serviceCodesReportItem.RefundsAmount;
                total.ReturnsAmount += serviceCodesReportItem.ReturnsAmount;
                total.TotalAmount += serviceCodesReportItem.TotalAmount;
                total.AmountPercent += serviceCodesReportItem.AmountPercent;
            }
            toReturn.Items.Add(total);


            return toReturn;
        }

        public async Task<PagedList<ServiceCodeRefundItem>> GetServiceCodeRefundItemsAsync(ServiceCodeItemsFilter filter)
        {
            PagedList<ServiceCodeRefundItem> toReturn = null;
            OrderQuery conditions = new OrderQuery().NotDeleted()
                .WithActualStatusOnly()
                .WithFromDate(filter.From)
                .WithToDate(filter.To)
                .WithRefundServiceCode(filter.ServiceCode);

            Func<IQueryLite<Order>, IQueryLite<Order>> include = x => x.Include(p => p.RefundSkus).
                ThenInclude(p => p.Sku).
                Include(p => p.OptionValues);
            Func<IQueryable<Order>, IOrderedQueryable<Order>> orderBy = p => p.OrderByDescending(pp => pp.Id);

            if (filter.Paging != null)
            {
                var refunds = await
                        _orderRefundService.SelectPageAsync(filter.Paging.PageIndex, filter.Paging.PageItemCount,
                            conditions,include, orderBy);
                toReturn = new PagedList<ServiceCodeRefundItem>()
                {
                    Count = refunds.Count,
                    Items = refunds.Items.Select(p => new ServiceCodeRefundItem(p)).ToList(),
                };
            }
            else
            {
                var refunds = await _orderRefundService.SelectAsync(conditions, include, orderBy: orderBy);
                toReturn = new PagedList<ServiceCodeRefundItem>()
                {
                    Count = refunds.Count,
                    Items = refunds.Select(p => new ServiceCodeRefundItem(p)).ToList(),
                };
            }

            var map = toReturn.Items.Where(p => p.IdOrderSource.HasValue).
                GroupBy(p => p.IdOrderSource).ToDictionary(p => p.Key, x => x.ToList());

            //load order source info
            int i = 0;
            var groups = toReturn.Items.GroupBy(x => i++ / SqlConstants.MAX_CONTAINS_COUNT).ToList();
            include = x => x.Include(p => p.OrderShippingPackages);
            foreach (var group in groups)
            {
                var ids = group.Select(p => p.IdOrderSource).Where(p => p.HasValue).Select(p => p.Value).Distinct().ToList();
                conditions = new OrderQuery().NotDeleted().WithIds(ids);
                var orderSources = await _orderService.SelectAsync(conditions, include);
                foreach (var orderSource in orderSources)
                {
                    List<ServiceCodeRefundItem> refunds;
                    if (map.TryGetValue(orderSource.Id, out refunds))
                    {
                        string warehouse = null;
                        string pWarehouse = null;
                        string npWarehouse = null;

                        if (orderSource.OrderShippingPackages != null && orderSource.OrderShippingPackages.Count > 0)
                        {
                            var package = orderSource.OrderShippingPackages.FirstOrDefault(p => !p.POrderType.HasValue);
                            if (package != null)
                            {
                                warehouse = Enum.GetName(typeof(Warehouse), package.IdWarehouse);
                            }

                            package = orderSource.OrderShippingPackages.FirstOrDefault(p => p.POrderType == (int)POrderType.P);
                            if (package != null)
                            {
                                pWarehouse = Enum.GetName(typeof(Warehouse), package.IdWarehouse);
                            }

                            package = orderSource.OrderShippingPackages.FirstOrDefault(p => p.POrderType == (int)POrderType.NP);
                            if (package != null)
                            {
                                npWarehouse = Enum.GetName(typeof(Warehouse), package.IdWarehouse);
                            }
                        }
                        foreach (var refund in refunds)
                        {
                            refund.OrderSourceDateCreated = orderSource.DateCreated;
                            refund.Warehouse = warehouse;
                            refund.PWarehouse = pWarehouse;
                            refund.NPWarehouse = npWarehouse;
                        }
                    }
                }
            }

            return toReturn;
        }

        public async Task<PagedList<ServiceCodeReshipItem>> GetServiceCodeReshipItemsAsync(ServiceCodeItemsFilter filter)
        {
            PagedList<ServiceCodeReshipItem> toReturn = null;
            OrderQuery conditions = new OrderQuery().NotDeleted()
                .WithActualStatusOnly()
                .WithFromDate(filter.From)
                .WithToDate(filter.To)
                .WithReshipServiceCode(filter.ServiceCode).WithOrderType(OrderType.Reship);

            Func<IQueryLite<Order>, IQueryLite<Order>> include = x =>
                x.Include(p => p.Skus).
                ThenInclude(p => p.Sku).
                Include(p=>p.Customer).
                Include(p=>p.ReshipProblemSkus).
                ThenInclude(p=>p.Sku);
            Func<IQueryable<Order>, IOrderedQueryable<Order>> orderBy = p => p.OrderByDescending(pp => pp.Id);

            var countries = await _countryService.GetCountriesAsync();
            
            if (filter.Paging != null)
            {
                var items =
                    await
                        _orderService.SelectPageAsync(filter.Paging.PageIndex, filter.Paging.PageItemCount, conditions,
                            include, orderBy);
                toReturn = new PagedList<ServiceCodeReshipItem>()
                {
                    Count = items.Count,
                    Items = items.Items.Select(p => new ServiceCodeReshipItem(p, countries)).ToList(),
                };
            }
            else
            {
                var items = await _orderService.SelectAsync(conditions, include, orderBy: orderBy);
                toReturn = new PagedList<ServiceCodeReshipItem>()
                {
                    Count = items.Count,
                    Items = items.Select(p => new ServiceCodeReshipItem(p, countries)).ToList(),
                };
            }
            var map = toReturn.Items.Where(p=>p.IdOrderSource.HasValue).
                GroupBy(p => p.IdOrderSource).ToDictionary(p => p.Key, x => x.ToList());

            //load order source info
            int i = 0;
            var groups = toReturn.Items.GroupBy(x => i++ / SqlConstants.MAX_CONTAINS_COUNT).ToList();
            include = x => x.Include(p => p.OrderShippingPackages).
                Include(p => p.ShippingAddress);
            foreach (var group in groups)
            {
                var ids = group.Select(p => p.IdOrderSource).Where(p=>p.HasValue).Select(p=>p.Value).Distinct().ToList();
                conditions = new OrderQuery().NotDeleted().WithIds(ids);
                var orderSources = await _orderService.SelectAsync(conditions,include);
                foreach (var orderSource in orderSources)
                {
                    List<ServiceCodeReshipItem> reships;
                    if (map.TryGetValue(orderSource.Id, out reships))
                    {
                        string shippingStateCode = null;
                        DateTime? shipDate = null;
                        string warehouse = null;
                        string carrier = null;
                        string shipService = null;

                        DateTime? pShipDate = null;
                        string pWarehouse = null;
                        string pCarrier = null;
                        string pShipService = null;

                        DateTime? npShipDate = null;
                        string npWarehouse = null;
                        string npCarrier = null;
                        string npShipService = null;
                        if (orderSource.ShippingAddress?.IdState.HasValue == true)
                        {
                            shippingStateCode = countries.SelectMany(p => p.States)
                                .FirstOrDefault(p => p.Id == orderSource.ShippingAddress?.IdState.Value)?
                                .StateCode;
                        }

                        if (orderSource.OrderShippingPackages != null && orderSource.OrderShippingPackages.Count > 0)
                        {
                            var package = orderSource.OrderShippingPackages.FirstOrDefault(p => !p.POrderType.HasValue);
                            if (package != null)
                            {
                                shipDate = package.ShippedDate;
                                warehouse = Enum.GetName(typeof(Warehouse), package.IdWarehouse);
                                carrier = package.ShipMethodFreightCarrier;
                                shipService = package.ShipMethodFreightService;
                            }

                            package = orderSource.OrderShippingPackages.FirstOrDefault(p => p.POrderType == (int)POrderType.P);
                            if (package != null)
                            {
                                pShipDate = package.ShippedDate;
                                pWarehouse = Enum.GetName(typeof(Warehouse), package.IdWarehouse);
                                pCarrier = package.ShipMethodFreightCarrier;
                                pShipService = package.ShipMethodFreightService;
                            }

                            package = orderSource.OrderShippingPackages.FirstOrDefault(p => p.POrderType == (int)POrderType.NP);
                            if (package != null)
                            {
                                npShipDate = package.ShippedDate;
                                npWarehouse = Enum.GetName(typeof(Warehouse), package.IdWarehouse);
                                npCarrier = package.ShipMethodFreightCarrier;
                                npShipService = package.ShipMethodFreightService;
                            }
                        }
                        foreach (var reship in reships)
                        {
                            if (orderSource.ShippingAddress?.IdState.HasValue == true)
                            {
                                reship.ShippingStateCode = shippingStateCode;

                                reship.ShipDate = shipDate;
                                reship.Warehouse = warehouse;
                                reship.Carrier = carrier;
                                reship.ShipService = shipService;

                                reship.PShipDate = pShipDate;
                                reship.PWarehouse = pWarehouse;
                                reship.PCarrier = pCarrier;
                                reship.PShipService = pShipService;

                                reship.NPShipDate = npShipDate;
                                reship.NPWarehouse = npWarehouse;
                                reship.NPCarrier = npCarrier;
                                reship.NPShipService = npShipService;
                            }
                        }
                    }
                }
            }

            return toReturn;
        }

        public async Task<bool> AssignServiceCodeForRefundsAsync(IEnumerable<int> ids, int serviceCode)
        {
            var refunds = await _orderRefundService.SelectAsync(p => ids.Contains(p.Id));
            refunds.ForEach(p=>p.Data.ServiceCode= serviceCode);
            await _orderRefundService.UpdateRangeAsync(refunds);
            return true;
        }

        public async Task<bool> AssignServiceCodeForReshipsAsync(IEnumerable<int> ids, int serviceCode)
        {
            var items = await _orderService.SelectAsync(p => ids.Contains(p.Id) && p.IdObjectType==(int)OrderType.Reship);
            items.ForEach(p => p.Data.ServiceCode = serviceCode);
            await _orderService.UpdateRangeAsync(items);
            return true;
        }
    }
}