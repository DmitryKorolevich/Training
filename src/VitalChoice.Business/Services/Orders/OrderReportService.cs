using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VitalChoice.Ecommerce.Domain.Entities;
using VitalChoice.Ecommerce.Domain.Entities.Orders;
using VitalChoice.Ecommerce.Domain.Exceptions;
using VitalChoice.Infrastructure.Domain.Dynamic;
using VitalChoice.Interfaces.Services;
using VitalChoice.Interfaces.Services.Customers;
using VitalChoice.Interfaces.Services.Orders;
using Microsoft.Extensions.Logging;
using VitalChoice.Business.Queries.Orders;
using VitalChoice.Business.Repositories;
using VitalChoice.Data.Extensions;
using VitalChoice.Data.Repositories;
using VitalChoice.Ecommerce.Domain.Transfer;
using VitalChoice.Infrastructure.Domain.Content.Articles;
using VitalChoice.Infrastructure.Domain.Entities;
using VitalChoice.Infrastructure.Domain.Entities.Users;
using VitalChoice.Infrastructure.Domain.Transfer.Orders;
using VitalChoice.Infrastructure.Domain.Transfer.Reports;
using VitalChoice.Data.Helpers;
using VitalChoice.DynamicData.Interfaces;
using VitalChoice.Ecommerce.Domain.Entities.Addresses;
using VitalChoice.Ecommerce.Domain.Entities.Customers;
using VitalChoice.Ecommerce.Domain.Entities.Discounts;
using VitalChoice.Ecommerce.Domain.Entities.Payment;
using VitalChoice.Ecommerce.Domain.Entities.Products;
using VitalChoice.Ecommerce.Domain.Helpers;
using VitalChoice.Infrastructure.Domain.Entities.Reports;
using VitalChoice.Interfaces.Services.Products;
using VitalChoice.Interfaces.Services.Settings;

namespace VitalChoice.Business.Services.Orders
{
    public class OrderReportService : IOrderReportService
    {
        private readonly OrderService _orderService;
        private readonly OrderRepository _orderRepository;
        private readonly IRepositoryAsync<AdminProfile> _adminProfileRepository;
        private readonly IRepositoryAsync<AdminTeam> _adminTeamRepository;
        private readonly SpEcommerceRepository _sPEcommerceRepository;
        private readonly ICountryService _countryService;
        private readonly IAppInfrastructureService _appInfrastructureService;
        private readonly IProductService _productService;
        private readonly IDiscountService _discountService;
        private readonly IDynamicMapper<AddressDynamic, Address> _addresMapper;
        private readonly ILogger _logger;

        public OrderReportService(
            OrderService orderService,
            OrderRepository orderRepository,
            IRepositoryAsync<AdminProfile> adminProfileRepository,
            IRepositoryAsync<AdminTeam> adminTeamRepository,
            SpEcommerceRepository sPEcommerceRepository,
            ICountryService countryService,
            IAppInfrastructureService appInfrastructureService,
            IProductService productService,
            IDiscountService discountService,
            IDynamicMapper<AddressDynamic, Address> addresMapper,
            ILoggerProviderExtended loggerProvider)
        {
            _orderService = orderService;
            _orderRepository = orderRepository;
            _adminProfileRepository = adminProfileRepository;
            _adminTeamRepository = adminTeamRepository;
            _sPEcommerceRepository = sPEcommerceRepository;
            _countryService = countryService;
            _appInfrastructureService = appInfrastructureService;
            _productService = productService;
            _discountService = discountService;
            _addresMapper = addresMapper;
            _logger = loggerProvider.CreateLogger<OrderReportService>();
        }

        public async Task<OrdersAgentReport> GetOrdersAgentReportAsync(OrdersAgentReportFilter filter)
        {
            OrdersAgentReport toReturn = new OrdersAgentReport();
            toReturn.IdAdminTeams = filter.IdAdminTeams;
            toReturn.IdAdmin = filter.IdAdmin;
            toReturn.FrequencyType = filter.FrequencyType;

            var agents = await _adminProfileRepository.Query(p => p.User.Status != UserStatus.Disabled).Include(p => p.User).SelectAsync(false);
            var teams = await _adminTeamRepository.Query().SelectAsync(false);

            List<int> specififcAgentIds = null;
            //if (filter.IdAdminTeam.HasValue)
            //{
            //    specififcAgentIds = agents.Where(p => p.IdAdminTeam == filter.IdAdminTeam.Value).Select(p => p.Id).ToList();
            //}
            //else if (filter.IdAdmin.HasValue)
            //{
            //    specififcAgentIds = agents.Where(p => p.Id == filter.IdAdmin.Value).Select(p => p.Id).ToList();
            //}
            specififcAgentIds = agents.Select(p => p.Id).ToList();

            var orders = await _orderRepository.GetOrdersForAgentReportAsync(filter.From, filter.To, specififcAgentIds);

            if (filter.From > filter.To)
            {
                return toReturn;
            }

            //create periods
            DateTime current = filter.From;
            if (filter.FrequencyType == FrequencyType.Monthly)
            {
                //start of next month
                current = new DateTime(current.Year, current.Month, 1, current.Hour, current.Minute, current.Second);
                current = current.AddMonths(1);
            }
            if (filter.FrequencyType == FrequencyType.Weekly)
            {
                //start of next week
                current = current.AddDays(-(int)current.DayOfWeek);
                current = current.AddDays(7);
            }
            if (filter.FrequencyType == FrequencyType.Daily)
            {
                current = current.AddDays(1);
            }
            if (current > filter.To)
            {
                current = filter.To;
            }
            var period = CreateAgentReportPeriod(filter.From, current, filter.IdAdmin, filter.IdAdminTeams, teams, agents);
            toReturn.Periods.Add(period);

            while (current < filter.To)
            {
                var nextCurrent = current;
                if (filter.FrequencyType == FrequencyType.Monthly)
                {
                    nextCurrent = nextCurrent.AddMonths(1);
                }
                if (filter.FrequencyType == FrequencyType.Weekly)
                {
                    nextCurrent = nextCurrent.AddDays(7);
                }
                if (filter.FrequencyType == FrequencyType.Daily)
                {
                    nextCurrent = nextCurrent.AddDays(1);
                }

                if (nextCurrent > filter.To)
                {
                    nextCurrent = filter.To;
                }

                period = CreateAgentReportPeriod(current, nextCurrent, filter.IdAdmin, filter.IdAdminTeams, teams, agents);
                toReturn.Periods.Add(period);
                current = nextCurrent;
            }

            //сombine orders info for periods
            decimal orderTotalWithoutShipping;
            foreach (var orderForAgentReport in orders)
            {
                orderTotalWithoutShipping = orderForAgentReport.Order.Total - orderForAgentReport.Order.ShippingTotal;
                orderTotalWithoutShipping = orderTotalWithoutShipping > 0 ? orderTotalWithoutShipping : 0;
                period = toReturn.Periods.FirstOrDefault(p => p.From <= orderForAgentReport.Order.DateCreated && p.To > orderForAgentReport.Order.DateCreated);
                if (period != null)
                {
                    if (orderForAgentReport.OrderType == SourceOrderType.Phone)
                    {
                        if (orderForAgentReport.Order.IdAddedBy.HasValue)
                        {
                            //phone orders for agent
                            var agent = period.Teams.SelectMany(p => p.Agents).FirstOrDefault(p => p.IdAdmin == orderForAgentReport.Order.IdAddedBy.Value);
                            if (agent != null)
                            {
                                if (orderForAgentReport.Order.IdObjectType == (int)OrderType.Refund)
                                {
                                    agent.RefundsCount++;
                                }
                                else if (orderForAgentReport.Order.IdObjectType == (int)OrderType.Reship)
                                {
                                    agent.ReshipsCount++;
                                }
                                else
                                {
                                    agent.OrdersCount++;
                                    agent.TotalOrdersAmount += orderTotalWithoutShipping;
                                    if (agent.HighestOrderAmount < orderTotalWithoutShipping)
                                    {
                                        agent.HighestOrderAmount = orderTotalWithoutShipping;
                                    }
                                    if (agent.LowestOrderAmount == 0 || (agent.LowestOrderAmount > orderTotalWithoutShipping && orderTotalWithoutShipping != 0))
                                    {
                                        agent.LowestOrderAmount = orderTotalWithoutShipping;
                                    }
                                }
                            }
                        }

                        //phone orders for period
                        if (orderForAgentReport.Order.IdObjectType == (int)OrderType.Refund)
                        {
                            period.RefundsCount++;
                        }
                        else if (orderForAgentReport.Order.IdObjectType == (int)OrderType.Reship)
                        {
                            period.ReshipsCount++;
                        }
                        else
                        {
                            period.OrdersCount++;
                            period.TotalOrdersAmount += orderTotalWithoutShipping;
                            if (period.HighestOrderAmount < orderTotalWithoutShipping)
                            {
                                period.HighestOrderAmount = orderTotalWithoutShipping;
                            }
                            if (period.LowestOrderAmount == 0 || (period.LowestOrderAmount > orderTotalWithoutShipping && orderTotalWithoutShipping != 0))
                            {
                                period.LowestOrderAmount = orderTotalWithoutShipping;
                            }
                        }
                    }

                    //all orders
                    if (orderForAgentReport.Order.IdObjectType == (int)OrderType.Refund)
                    {
                        period.AllRefundsCount++;
                    }
                    else if (orderForAgentReport.Order.IdObjectType == (int)OrderType.Reship)
                    {
                        period.AllReshipsCount++;
                    }
                    else
                    {
                        period.AllOrdersCount++;
                        period.AllTotalOrdersAmount += orderTotalWithoutShipping;
                        if (period.AllHighestOrderAmount < orderTotalWithoutShipping)
                        {
                            period.AllHighestOrderAmount = orderTotalWithoutShipping;
                        }
                        if (period.AllLowestOrderAmount == 0 || (period.AllLowestOrderAmount > orderTotalWithoutShipping && orderTotalWithoutShipping != 0))
                        {
                            period.AllLowestOrderAmount = orderTotalWithoutShipping;
                        }
                    }
                }
            }

            //calculate teams data and percents
            foreach (var ordersAgentReportPeriodItem in toReturn.Periods)
            {
                ordersAgentReportPeriodItem.AverageOrdersAmount = ordersAgentReportPeriodItem.OrdersCount != 0
                    ? ordersAgentReportPeriodItem.TotalOrdersAmount / ordersAgentReportPeriodItem.OrdersCount
                    : 0;
                ordersAgentReportPeriodItem.AllAverageOrdersAmount = ordersAgentReportPeriodItem.AllOrdersCount != 0
                    ? ordersAgentReportPeriodItem.AllTotalOrdersAmount / ordersAgentReportPeriodItem.AllOrdersCount
                    : 0;
                ordersAgentReportPeriodItem.AgentOrdersPercent = ordersAgentReportPeriodItem.TotalOrdersAmount != 0 ?
                    Math.Round(ordersAgentReportPeriodItem.TotalOrdersAmount * 100 / ordersAgentReportPeriodItem.AllTotalOrdersAmount, 2)
                    : 0;
                ordersAgentReportPeriodItem.AverageOrdersAmountDifference = ordersAgentReportPeriodItem.AverageOrdersAmount - ordersAgentReportPeriodItem.AllAverageOrdersAmount;

                foreach (var ordersAgentReportTeamItem in ordersAgentReportPeriodItem.Teams)
                {
                    foreach (var ordersAgentReportAgentItem in ordersAgentReportTeamItem.Agents)
                    {
                        ordersAgentReportAgentItem.AverageOrdersAmount = ordersAgentReportAgentItem.OrdersCount != 0
                            ? ordersAgentReportAgentItem.TotalOrdersAmount / ordersAgentReportAgentItem.OrdersCount
                            : 0;

                        ordersAgentReportTeamItem.OrdersCount += ordersAgentReportAgentItem.OrdersCount;
                        ordersAgentReportTeamItem.TotalOrdersAmount += ordersAgentReportAgentItem.TotalOrdersAmount;
                        ordersAgentReportTeamItem.ReshipsCount += ordersAgentReportAgentItem.ReshipsCount;
                        ordersAgentReportTeamItem.RefundsCount += ordersAgentReportAgentItem.RefundsCount;

                        if (ordersAgentReportTeamItem.HighestOrderAmount < ordersAgentReportAgentItem.HighestOrderAmount)
                        {
                            ordersAgentReportTeamItem.HighestOrderAmount = ordersAgentReportAgentItem.HighestOrderAmount;
                        }
                        if (ordersAgentReportTeamItem.LowestOrderAmount == 0 || (ordersAgentReportTeamItem.LowestOrderAmount > ordersAgentReportAgentItem.LowestOrderAmount &&
                            ordersAgentReportAgentItem.LowestOrderAmount != 0))
                        {
                            ordersAgentReportTeamItem.LowestOrderAmount = ordersAgentReportAgentItem.LowestOrderAmount;
                        }
                    }

                    ordersAgentReportTeamItem.AverageOrdersAmount = ordersAgentReportTeamItem.OrdersCount != 0
                        ? ordersAgentReportTeamItem.TotalOrdersAmount / ordersAgentReportTeamItem.OrdersCount
                        : 0;

                    ordersAgentReportTeamItem.AgentOrdersPercent = ordersAgentReportTeamItem.TotalOrdersAmount != 0
                        ? Math.Round(ordersAgentReportTeamItem.TotalOrdersAmount * 100 / ordersAgentReportPeriodItem.TotalOrdersAmount, 2)
                        : 0;
                    ordersAgentReportTeamItem.AverageOrdersAmountDifference = ordersAgentReportTeamItem.AverageOrdersAmount - ordersAgentReportPeriodItem.AverageOrdersAmount;
                }
            }

            return toReturn;
        }

        public ICollection<OrdersAgentReportExportItem> ConvertOrdersAgentReportToExportItems(OrdersAgentReport report, bool fullReport)
        {
            List<OrdersAgentReportExportItem> toReturn = new List<OrdersAgentReportExportItem>();
            foreach (var period in report.Periods)
            {
                var item = new OrdersAgentReportExportItem();
                if (report.FrequencyType == FrequencyType.Daily)
                {
                    item.Agent = $"{period.From:MM/dd/yy}";
                }
                else
                {

                    item.Agent = $"{period.From:MM/dd/yy} - {period.To:MM/dd/yy}";
                }
                toReturn.Add(item);
                toReturn.Add(new OrdersAgentReportExportItem());
                foreach (var team in period.Teams)
                {
                    if (!string.IsNullOrEmpty(team.AdminTeamName))
                    {
                        toReturn.Add(new OrdersAgentReportExportItem()
                        {
                            Agent = team.AdminTeamName
                        });
                    }

                    if (team.Agents.Count > 0)
                    {
                        toReturn.Add(new OrdersAgentReportExportItem()
                        {
                            Agent = "Agent",
                            OrdersCount = "# of Orders",
                            TotalOrdersAmount = "Total Order Value",
                            AverageOrdersAmount = "Average Order Value",
                            LowestOrderAmount = "Lowest Order Value",
                            HighestOrderAmount = "Highest Order Value",
                            RefundsCount = "# Refunds",
                            ReshipsCount = "# Reships",
                        });
                    }
                    foreach (var agent in team.Agents)
                    {
                        toReturn.Add(new OrdersAgentReportExportItem()
                        {
                            Agent = agent.AgentId,
                            OrdersCount = agent.OrdersCount.ToString(),
                            TotalOrdersAmount = agent.TotalOrdersAmount.ToString("C2"),
                            AverageOrdersAmount = agent.AverageOrdersAmount.ToString("C2"),
                            LowestOrderAmount = agent.LowestOrderAmount.ToString("C2"),
                            HighestOrderAmount = agent.HighestOrderAmount.ToString("C2"),
                            RefundsCount = agent.RefundsCount.ToString(),
                            ReshipsCount = agent.ReshipsCount.ToString(),
                        });
                    }

                    if (fullReport && !report.IdAdmin.HasValue)
                    {
                        toReturn.Add(new OrdersAgentReportExportItem()
                        {
                            Agent = "Team Total",
                            OrdersCount = team.OrdersCount.ToString(),
                            TotalOrdersAmount = team.TotalOrdersAmount.ToString("C2"),
                            AverageOrdersAmount = team.AverageOrdersAmount.ToString("C2"),
                            LowestOrderAmount = team.LowestOrderAmount.ToString("C2"),
                            HighestOrderAmount = team.HighestOrderAmount.ToString("C2"),
                            RefundsCount = team.RefundsCount.ToString(),
                            ReshipsCount = team.ReshipsCount.ToString(),
                        });
                    }

                    if (fullReport)
                    {
                        team.AgentOrdersPercent = team.AgentOrdersPercent != 0 ? team.AgentOrdersPercent / 100 : 0;
                        toReturn.Add(new OrdersAgentReportExportItem()
                        {
                            Agent = "% of total phone orders",
                            OrdersCount = $"{team.AgentOrdersPercent:P2}",
                            AverageOrdersAmount = team.AverageOrdersAmountDifference.ToString("C2"),
                        });
                    }

                    toReturn.Add(new OrdersAgentReportExportItem());
                }

                if (fullReport)
                {
                    toReturn.Add(new OrdersAgentReportExportItem()
                    {
                        Agent = "Overall Teams Total(phone orders)",
                        OrdersCount = period.OrdersCount.ToString(),
                        TotalOrdersAmount = period.TotalOrdersAmount.ToString("C2"),
                        AverageOrdersAmount = period.AverageOrdersAmount.ToString("C2"),
                        LowestOrderAmount = period.LowestOrderAmount.ToString("C2"),
                        HighestOrderAmount = period.HighestOrderAmount.ToString("C2"),
                        RefundsCount = period.RefundsCount.ToString(),
                        ReshipsCount = period.ReshipsCount.ToString(),
                    });
                    period.AgentOrdersPercent = period.AgentOrdersPercent != 0 ? period.AgentOrdersPercent/100 : 0;
                    toReturn.Add(new OrdersAgentReportExportItem()
                    {
                        Agent = "% of total orders",
                        OrdersCount = $"{period.AgentOrdersPercent:P2}",
                        AverageOrdersAmount = period.AverageOrdersAmountDifference.ToString("C2"),
                    });
                    toReturn.Add(new OrdersAgentReportExportItem()
                    {
                        Agent = "Total Orders",
                        OrdersCount = period.AllOrdersCount.ToString(),
                        TotalOrdersAmount = period.AllTotalOrdersAmount.ToString("C2"),
                        AverageOrdersAmount = period.AllAverageOrdersAmount.ToString("C2"),
                        LowestOrderAmount = period.AllLowestOrderAmount.ToString("C2"),
                        HighestOrderAmount = period.AllHighestOrderAmount.ToString("C2"),
                        RefundsCount = period.AllRefundsCount.ToString(),
                        ReshipsCount = period.AllReshipsCount.ToString(),
                    });

                    toReturn.Add(new OrdersAgentReportExportItem());
                }
            }

            return toReturn;
        }

        private OrdersAgentReportPeriodItem CreateAgentReportPeriod(DateTime from, DateTime to, int? idAdmin, ICollection<int> idAdminTeams, List<AdminTeam> teams,
            List<AdminProfile> agents)
        {
            OrdersAgentReportPeriodItem period = new OrdersAgentReportPeriodItem();
            period.From = from;
            period.To = to;
            if (!idAdmin.HasValue)
            {
                var currentTeams = idAdminTeams!=null && idAdminTeams.Count>0
                    ? teams.Where(p => idAdminTeams.Contains(p.Id)).ToList()
                    : teams;
                foreach (var team in currentTeams)
                {
                    OrdersAgentReportTeamItem teamItem = new OrdersAgentReportTeamItem();
                    teamItem.IdAdminTeam = team.Id;
                    teamItem.AdminTeamName = team.Name;
                    teamItem.Agents = agents.Where(p => p.IdAdminTeam == team.Id).Select(p => new OrdersAgentReportAgentItem()
                    {
                        IdAdmin = p.Id,
                        AgentId = p.AgentId,
                    }).OrderBy(p => p.AgentId).ToList();
                    period.Teams.Add(teamItem);
                }
                //admins without team
                if (idAdminTeams == null || idAdminTeams.Count == 0)
                {
                    OrdersAgentReportTeamItem teamItem = new OrdersAgentReportTeamItem();
                    teamItem.AdminTeamName = "Not Specified";
                    teamItem.Agents = agents.Where(p => !p.IdAdminTeam.HasValue).Select(p => new OrdersAgentReportAgentItem()
                    {
                        IdAdmin = p.Id,
                        AgentId = p.AgentId,
                    }).OrderBy(p => p.AgentId).ToList();
                    period.Teams.Add(teamItem);
                }
            }
            else
            {
                OrdersAgentReportTeamItem teamItem = new OrdersAgentReportTeamItem();
                teamItem.Agents = agents.Where(p => p.Id == idAdmin.Value).Select(p => new OrdersAgentReportAgentItem()
                {
                    IdAdmin = p.Id,
                    AgentId = p.AgentId,
                }).ToList();
                period.Teams.Add(teamItem);
            }
            return period;
        }

        public async Task<WholesaleDropShipReport> GetWholesaleDropShipReportAsync(WholesaleDropShipReportFilter filter)
        {
            WholesaleDropShipReport toReturn=new WholesaleDropShipReport();

            var skus = await _sPEcommerceRepository.GetWholesaleDropShipReportSkusSummaryAsync(filter);
            if (skus.Count > 0)
            {
                var sku = skus.First();
                toReturn.DiscountedSubtotal = sku.ProductsSubtotal - sku.DiscountTotal;
                toReturn.Shipping = sku.ShippingTotal;
                toReturn.Total = sku.Total;
                toReturn.Skus = skus.Select(p => new WholesaleDropShipReportSkuSummary()
                {
                    Id = p.Id,
                    Code = p.Code,
                    Amount = p.Amount,
                    Quantity = p.Quantity,
                }).ToList();

                toReturn.SkusTotal=new WholesaleDropShipReportSkuSummary()
                {
                    Amount = toReturn.Skus.Sum(p=>p.Amount),
                    Quantity = toReturn.Skus.Sum(p => p.Quantity),
                };
            }

            return toReturn;
        }

        public async Task<PagedList<WholesaleDropShipReportOrderItem>> GetOrdersForWholesaleDropShipReportAsync(WholesaleDropShipReportFilter filter)
        {
            PagedList<WholesaleDropShipReportOrderItem> toReturn = new PagedList<WholesaleDropShipReportOrderItem>();

            var countries = await _countryService.GetCountriesAsync();

            toReturn.Count= await _sPEcommerceRepository.GetCountOrderIdsForWholesaleDropShipReportAsync(filter);
            var ids = await _sPEcommerceRepository.GetOrderIdsForWholesaleDropShipReportAsync(filter);
            var orders = await _orderService.SelectAsync(ids, includesOverride: x => x
                .Include(o => o.Skus)
                .ThenInclude(s => s.Sku)
                .Include(o => o.PromoSkus)
                .ThenInclude(p => p.Sku)
                .Include(o => o.ShippingAddress)
                .ThenInclude(s => s.OptionValues));

            orders.ForEach(p =>
            {
                WholesaleDropShipReportOrderItem item = new WholesaleDropShipReportOrderItem();
                item.IdOrder = p.Id;
                item.OrderStatus = p.OrderStatus;
                item.POrderStatus = p.POrderStatus;
                item.NPOrderStatus = p.NPOrderStatus;
                item.DiscountedSubtotal = p.ProductsSubtotal - p.DiscountTotal;
                item.Shipping = p.ShippingTotal;
                item.Total = p.Total;
                item.OrderNotes = p.SafeData.OrderNotes;
                item.PoNumber = p.SafeData.PoNumber;
                item.ShippingCompany = p.ShippingAddress?.SafeData.Company;
                item.ShippingFirstName = p.ShippingAddress?.SafeData.FirstName;
                item.ShippingLastName = p.ShippingAddress?.SafeData.LastName;
                item.ShippingAddress1 = p.ShippingAddress?.SafeData.Address1;
                item.ShippingAddress1 = p.ShippingAddress?.SafeData.Address2;
                item.Zip = p.ShippingAddress?.SafeData.Zip;
                item.City = p.ShippingAddress?.SafeData.City;
                item.Country = countries.FirstOrDefault(x=>x.Id==p.ShippingAddress?.IdCountry)?.CountryCode;
                item.StateCode = countries.SelectMany(x=>x.States).FirstOrDefault(x => x.Id == p.ShippingAddress?.IdState)?.StateCode;
                item.Phone = p.ShippingAddress?.SafeData.Phone;

                item.Skus = p.Skus.Select(x => new WholesaleDropShipReportSkuItem()
                {
                    Id = x.Sku?.Id ?? 0,
                    Code = x.Sku?.Code,
                    Price = x.Amount,
                    Quantity = x.Quantity,
                }).ToList();

                item.Skus.AddRange(p.PromoSkus.Select(x => new WholesaleDropShipReportSkuItem()
                {
                    Id = x.Sku?.Id ?? 0,
                    Code = x.Sku?.Code,
                    Price = x.Amount,
                    Quantity = x.Quantity,
                }));

                toReturn.Items.Add(item);
            });

            return toReturn;
        }

        public async Task<PagedList<TransactionAndRefundReportItem>> GetTransactionAndRefundReportItemsAsync(TransactionAndRefundReportFilter filter)
        {
            PagedList<TransactionAndRefundReportItem> toReturn = new PagedList<TransactionAndRefundReportItem>();

            var dbItems = await _sPEcommerceRepository.GetTransactionAndRefundReportItemsAsync(filter);

            dbItems.ForEach(p =>
            {
                TransactionAndRefundReportItem item = new TransactionAndRefundReportItem();
                item.IdOrder = p.IdOrder;
                item.IdOrderSource = p.IdOrderSource;
                item.Rank = p.Rank;
                item.IdObjectType = (OrderType)p.IdObjectType;
                item.OrderStatus = (OrderStatus?) p.OrderStatus;
                item.POrderStatus = (OrderStatus?)p.POrderStatus;
                item.NPOrderStatus = (OrderStatus?)p.NPOrderStatus;
                item.ServiceCode = p.ServiceCode;
                item.ServiceCodeName =  p.ServiceCode.HasValue ? _appInfrastructureService.Data().ServiceCodes.FirstOrDefault(pp=>p.ServiceCode.Value==pp.Key)?.Text : null;
                item.IdCustomer = p.IdCustomer;
                item.CustomerFirstName = p.CustomerFirstName;
                item.CustomerLastName = p.CustomerLastName;
                item.CustomerIdObjectType = (CustomerType)p.CustomerIdObjectType;
                item.ProductsSubtotal = p.ProductsSubtotal;
                item.DiscountTotal = p.DiscountTotal;
                item.DiscountedSubtotal = p.IdObjectType == (int) OrderType.Refund ? -(p.ProductsSubtotal - p.DiscountTotal) : p.ProductsSubtotal - p.DiscountTotal;
                item.ShippingTotal = p.ShippingTotal;
                item.TaxTotal = p.TaxTotal;
                item.Total = p.IdObjectType == (int)OrderType.Refund ? -p.Total : p.Total;
                item.ReturnAssociated = p.ReturnAssociated;
                item.PaymentMethodIdObjectType = (PaymentMethodType?)p.PaymentMethodIdObjectType;
                item.DiscountIdObjectType = (DiscountType?)p.DiscountIdObjectType;
                item.DiscountPercent = p.DiscountPercent;
                item.IdSku = p.IdSku;
                item.IdProduct = p.IdProduct;
                item.Code = p.Code;
                item.DisplayName = p.ProductName;
                if (!string.IsNullOrEmpty(p.ProductSubTitle))
                {
                    item.DisplayName += " " + p.ProductSubTitle;
                }
                item.DisplayName += $" ({p.SkuQTY})";
                item.OrderQuantity = p.OrderQuantity;
                item.ProductIdObjectType = (ProductType?)p.ProductIdObjectType;
                item.Price = p.IdObjectType==(int)OrderType.Refund ? -p.Price : p.Price;
                item.RefundIdRedeemType = (RedeemType?)p.RefundIdRedeemType;
                item.RefundProductPercent = p.RefundProductPercent;
                item.Override = "no";

                toReturn.Items.Add(item);
            });
            toReturn.Count = dbItems.Count > 0 ? dbItems.First().TotalCount : 0;

            return toReturn;
        }

        public async Task<ICollection<OrdersSummarySalesOrderTypeStatisticItem>> GetOrdersSummarySalesOrderTypeStatisticItemsAsync(OrdersSummarySalesReportFilter filter)
        {
            var toReturn = await _sPEcommerceRepository.GetOrdersSummarySalesOrderTypeStatisticItemsAsync(filter);

            var total = new OrdersSummarySalesOrderTypeStatisticItem();
            total.Name = "Total";
            toReturn.ForEach(p =>
            {
                p.Name = _appInfrastructureService.Data().OrderSourceTypes.FirstOrDefault(pp => p.Id == pp.Key)?.Text;
                p.Average = p.Count != 0 ? p.Total/p.Count : 0;
                total.Count += p.Count;
                total.Total += p.Total;
            });
            total.Average = total.Count != 0 ? total.Total / total.Count : 0;
            toReturn.Add(total);

            return toReturn;
        }

        public async Task<PagedList<OrdersSummarySalesOrderItem>> GetOrdersSummarySalesOrderItemsAsync(OrdersSummarySalesReportFilter filter)
        {
            PagedList<OrdersSummarySalesOrderItem> toReturn = new PagedList<OrdersSummarySalesOrderItem>();

            var items = await _sPEcommerceRepository.GetOrdersSummarySalesOrderItemsAsync(filter);

            items.ForEach(p =>
            {
                p.SourceName = p.Source.HasValue ? _appInfrastructureService.Data().OrderSources.FirstOrDefault(pp => p.Source.Value == pp.Key)?.Text : null;
            });
            toReturn.Items = items.ToList();
            toReturn.Count = items.Count > 0 ? items.First().TotalCount : 0;

            return toReturn;
        }

        public async Task<PagedList<SkuAddressReportItem>> GetSkuAddressReportItemsAsync(SkuAddressReportFilter filter)
        {
            var toReturn = new PagedList<SkuAddressReportItem>();
            int? idSku = null;
            int? idDiscount = null;
            if (!string.IsNullOrEmpty(filter.SkuCode))
            {
                idSku = (await _productService.GetSkuAsync(filter.SkuCode))?.Id;
                if (!idSku.HasValue)
                {
                    return toReturn;
                }
            }
            if (!string.IsNullOrEmpty(filter.DiscountCode))
            {
                idDiscount = (await _discountService.GetByCode(filter.DiscountCode))?.Id;
                if (!idDiscount.HasValue)
                {
                    return toReturn;
                }
            }

            OrderQuery conditions = new OrderQuery().NotDeleted().WithCreatedDate(filter.From, filter.To).WithActualStatusOnly().
                    WithOrderTypes(new [] { OrderType.AutoShipOrder, OrderType.DropShip, OrderType.GiftList, OrderType.Normal }).
                    WithCustomerType(filter.IdCustomerType).WithIdSku(idSku).WithIdDiscount(idDiscount,filter.WithoutDiscount);

            Func<IQueryable<Order>, IOrderedQueryable<Order>> sortable = x => x.OrderByDescending(y => y.Id);
            Func<IQueryLite<Order>, IQueryLite<Order>> includes = p => p.Include(c => c.OptionValues).
                Include(c => c.PaymentMethod).ThenInclude(c => c.BillingAddress).ThenInclude(c => c.OptionValues).
                Include(c => c.ShippingAddress).ThenInclude(c => c.OptionValues).
                Include(c => c.Customer).ThenInclude(c => c.OptionValues).
                Include(c => c.Skus).ThenInclude(c => c.Sku).
                Include(c => c.PromoSkus).ThenInclude(c => c.Sku).
                Include(c => c.Discount);

            PagedList<OrderDynamic> orders;
            if(filter.Paging!=null)
            { 
                orders = await _orderService.SelectPageAsync(filter.Paging.PageIndex, filter.Paging.PageItemCount, conditions,
                        includes, orderBy: sortable, withDefaults: true);
            }
            else
            {
                var data = await _orderService.SelectAsync(conditions, includes, orderBy: sortable, withDefaults: true);
                orders =new PagedList<OrderDynamic>();
                orders.Items = data;
                orders.Count = data.Count;
            }

            toReturn.Count = orders.Count;

            var countries = await _countryService.GetCountriesAsync();

            foreach (var order in orders.Items)
            {
                foreach (var skuOrdered in order.Skus)
                {
                    if (!idSku.HasValue || skuOrdered.Sku.Id == idSku.Value)
                    {
                        SkuAddressReportItem item = new SkuAddressReportItem();
                        SetBaseSkuAddressReportItemFields(item, order, skuOrdered, countries);
                        toReturn.Items.Add(item);
                    }
                }

                foreach (var skuOrdered in order.PromoSkus.Where(p=>p.Enabled))
                {
                    if (!idSku.HasValue || skuOrdered.Sku.Id == idSku.Value)
                    {
                        SkuAddressReportItem item = new SkuAddressReportItem();
                        SetBaseSkuAddressReportItemFields(item, order, skuOrdered, countries);
                        toReturn.Items.Add(item);
                    }
                }
            }

            return toReturn;
        }

        private void SetBaseSkuAddressReportItemFields(SkuAddressReportItem item, OrderDynamic order, SkuOrdered skuOrdered, ICollection<Country> countries)
        {
            item.IdOrder = order.Id;
            item.DateCreated = order.DateCreated;
            item.IdObjectType = order.IdObjectType;
            item.OrderStatus = order.OrderStatus;
            item.POrderStatus = order.POrderStatus;
            item.NPOrderStatus = order.NPOrderStatus;
            item.IdCustomer = order.Customer.Id;
            item.IdCustomerObjectType = order.IdObjectType;

            item.SkuCode = skuOrdered.Sku.Code;
            item.DiscountCode = order.Discount?.Code;
            item.Quantity = skuOrdered.Quantity;
            item.Price = skuOrdered.Amount;
            item.Amount = skuOrdered.Quantity*skuOrdered.Amount;

            item.Source = _appInfrastructureService.Data().OrderSources.FirstOrDefault(p => (order.Customer.SafeData.Source ?? 0) == p.Key)?.Text;
            item.DoNotMail = order.Customer.SafeData.DoNotMail ?? false;

            var shippingAddress = _addresMapper.ToModelAsync<SkuAddressReportAddressItem>(order.ShippingAddress).Result;
            shippingAddress.CountyCode = countries.FirstOrDefault(x => x.Id == shippingAddress.IdCountry)?.CountryCode;
            shippingAddress.StateCode = countries.SelectMany(x => x.States).FirstOrDefault(x => x.Id == shippingAddress.IdState)?.StateCode;

            item.ShippingFirstName = shippingAddress.FirstName;
            item.ShippingLastName = shippingAddress.LastName;
            item.ShippingCompany = shippingAddress.Company;
            item.ShippingAddress1 = shippingAddress.Address1;
            item.ShippingAddress2 = shippingAddress.Address2;
            item.ShippingCity = shippingAddress.City;
            item.ShippingCounty = shippingAddress.County;
            item.ShippingCountyCode = shippingAddress.CountyCode;
            item.ShippingStateCode = shippingAddress.StateCode;
            item.ShippingZip = shippingAddress.Zip;
            item.ShippingPhone = shippingAddress.Phone;

            SkuAddressReportAddressItem billingAddress;
            if (order.PaymentMethod.Address != null)
            {
                billingAddress = _addresMapper.ToModelAsync<SkuAddressReportAddressItem>(order.PaymentMethod.Address).Result;
                billingAddress.CountyCode = countries.FirstOrDefault(x => x.Id == order.PaymentMethod.Address?.IdCountry)?.CountryCode;
                billingAddress.StateCode = countries.SelectMany(x => x.States).FirstOrDefault(x => x.Id == order.PaymentMethod.Address?.IdState)?.StateCode;
            }
            else
            {
                billingAddress = new SkuAddressReportAddressItem();
            }

            item.BillingFirstName = shippingAddress.FirstName;
            item.BillingLastName = shippingAddress.LastName;
            item.BillingCompany = shippingAddress.Company;
            item.BillingAddress1 = shippingAddress.Address1;
            item.BillingAddress2 = shippingAddress.Address2;
            item.BillingCity = shippingAddress.City;
            item.BillingCounty = shippingAddress.County;
            item.BillingCountyCode = shippingAddress.CountyCode;
            item.BillingStateCode = shippingAddress.StateCode;
            item.BillingZip = shippingAddress.Zip;
            item.BillingPhone = shippingAddress.Phone;
        }
    }
}