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
using VitalChoice.Data.Repositories;
using VitalChoice.Infrastructure.Domain.Content.Articles;
using VitalChoice.Infrastructure.Domain.Entities;
using VitalChoice.Infrastructure.Domain.Entities.Users;
using VitalChoice.Infrastructure.Domain.Transfer.Orders;

namespace VitalChoice.Business.Services.Orders
{
    public class OrderReportService : IOrderReportService
    {
        private readonly OrderService _orderService;
        private readonly OrderRepository _orderRepository;
        private readonly IRepositoryAsync<AdminProfile> _adminProfileRepository;
        private readonly IRepositoryAsync<AdminTeam> _adminTeamRepository;
        private readonly ILogger _logger;

        public OrderReportService(
            OrderService orderService,
            OrderRepository orderRepository,
            IRepositoryAsync<AdminProfile> adminProfileRepository,
            IRepositoryAsync<AdminTeam> adminTeamRepository,
            ILoggerProviderExtended loggerProvider)
        {
            _orderService = orderService;
            _orderRepository = orderRepository;
            _adminProfileRepository = adminProfileRepository;
            _adminTeamRepository = adminTeamRepository;
            _logger = loggerProvider.CreateLoggerDefault();
        }

        public async Task<OrdersAgentReport> GetOrdersAgentReportAsync(OrdersAgentReportFilter filter)
        {
            OrdersAgentReport toReturn = new OrdersAgentReport();

            var agents = await _adminProfileRepository.Query().SelectAsync(false);
            var teams = await _adminTeamRepository.Query().SelectAsync(false);

            List<int> specififcAgentIds = null;
            if (filter.IdAdminTeam.HasValue)
            {
                specififcAgentIds = agents.Where(p => p.IdAdminTeam == filter.IdAdminTeam.Value).Select(p => p.Id).ToList();
            }
            else if (filter.IdAdmin.HasValue)
            {
                specififcAgentIds = agents.Where(p => p.Id == filter.IdAdmin.Value).Select(p => p.Id).ToList();
            }

            var orders = await _orderRepository.GetOrdersForAgentReportAsync(filter.From, filter.To, specififcAgentIds);

            if (filter.From < filter.To)
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
            var period = CreateAgentReportPeriod(filter.From, current, filter.IdAdmin, filter.IdAdminTeam, teams, agents, filter.FrequencyType);
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

                period = CreateAgentReportPeriod(current, nextCurrent, filter.IdAdmin, filter.IdAdminTeam, teams, agents, filter.FrequencyType);
                toReturn.Periods.Add(period);
                current = nextCurrent;
            }

            //сombine orders info for periods
            decimal orderTotalWithoutShipping;
            foreach (var orderForAgentReport in orders)
            {
                orderTotalWithoutShipping = orderForAgentReport.Order.Total - orderForAgentReport.Order.ShippingTotal;
                period = toReturn.Periods.FirstOrDefault(p=>p.From>= orderForAgentReport.Order.DateCreated && p.To<orderForAgentReport.Order.DateCreated);
                if (period != null)
                {
                    if (orderForAgentReport.OrderType == SourceOrderType.Phone)
                    {
                        if(orderForAgentReport.Order.IdAddedBy.HasValue)
                        { 
                            //phone orders for agent
                            var agent=period.Teams.SelectMany(p=>p.Agents).FirstOrDefault(p=>p.IdAdmin== orderForAgentReport.Order.IdAddedBy.Value);
                            if (agent != null)
                            {
                                if (orderForAgentReport.Order.IdObjectType == (int) OrderType.Refund)
                                {
                                    agent.RefundsCount++;
                                }
                                else if(orderForAgentReport.Order.IdObjectType == (int)OrderType.Reship)
                                {
                                    agent.ReshipsCount++;
                                }
                                else
                                {
                                    agent.OrdersCount++;
                                    agent.TotalOrdersAmount = orderTotalWithoutShipping;
                                    if (agent.HighestOrderAmount < orderTotalWithoutShipping)
                                    {
                                        agent.HighestOrderAmount = orderTotalWithoutShipping;
                                    }
                                    if (agent.LowestOrderAmount==0 || (agent.LowestOrderAmount > orderTotalWithoutShipping && orderTotalWithoutShipping!=0))
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
                            period.TotalOrdersAmount = orderTotalWithoutShipping;
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
                        period.AllTotalOrdersAmount = orderTotalWithoutShipping;
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
                ordersAgentReportPeriodItem.AgentOrdersPercent = ordersAgentReportPeriodItem.TotalOrdersAmount / period.AllTotalOrdersAmount;
                ordersAgentReportPeriodItem.AverageOrdersAmountDifference = ordersAgentReportPeriodItem.AverageOrdersAmountDifference - ordersAgentReportPeriodItem.AllAverageOrdersAmount;

                foreach (var ordersAgentReportTeamItem in ordersAgentReportPeriodItem.Teams)
                {
                    foreach (var ordersAgentReportAgentItem in ordersAgentReportTeamItem.Agents)
                    {
                        ordersAgentReportAgentItem.AverageOrdersAmount = ordersAgentReportAgentItem.OrdersCount != 0
                            ? ordersAgentReportAgentItem.TotalOrdersAmount/ordersAgentReportAgentItem.OrdersCount
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

                    ordersAgentReportTeamItem.AgentOrdersPercent = ordersAgentReportTeamItem.TotalOrdersAmount/ period.TotalOrdersAmount;
                    ordersAgentReportTeamItem.AverageOrdersAmountDifference = ordersAgentReportTeamItem.AverageOrdersAmountDifference - ordersAgentReportPeriodItem.AverageOrdersAmount;
                }
            }

            return toReturn;
        }

        private OrdersAgentReportPeriodItem CreateAgentReportPeriod(DateTime from, DateTime to,int? idAdmin, int? idAdminTeam, List<AdminTeam> teams,
            List<AdminProfile> agents, FrequencyType frequencyType)
        {
            OrdersAgentReportPeriodItem period = new OrdersAgentReportPeriodItem();
            period.FrequencyType = frequencyType;
            period.From = from;
            period.To = to;
            if (!idAdmin.HasValue)
            {
                var currentTeams = idAdminTeam.HasValue
                    ? teams.Where(p => p.Id == idAdminTeam.Value).ToList()
                    : teams;
                foreach (var team in currentTeams)
                {
                    OrdersAgentReportTeamItem teamItem = new OrdersAgentReportTeamItem();
                    teamItem.IdAdminTeam = team.Id;
                    teamItem.Agents = agents.Where(p => p.IdAdminTeam == team.Id).Select(p => new OrdersAgentReportAgentItem()
                    {
                        IdAdmin = p.Id,
                        AgentId = p.AgentId,
                    }).ToList();
                    period.Teams.Add(teamItem);
                }
                //admins without team
                if (!idAdminTeam.HasValue)
                {
                    OrdersAgentReportTeamItem teamItem = new OrdersAgentReportTeamItem();
                    teamItem.Agents = agents.Where(p => !p.IdAdminTeam.HasValue).Select(p => new OrdersAgentReportAgentItem()
                    {
                        IdAdmin = p.Id,
                        AgentId = p.AgentId,
                    }).ToList();
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
    }
}