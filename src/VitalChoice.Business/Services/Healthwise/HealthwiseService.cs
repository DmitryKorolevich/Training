using Microsoft.AspNet.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VitalChoice.Business.Queries.Customer;
using VitalChoice.Business.Queries.Healthwise;
using VitalChoice.Business.Queries.Orders;
using VitalChoice.Business.Repositories;
using VitalChoice.Business.Services.Dynamic;
using VitalChoice.Business.Services.Ecommerce;
using VitalChoice.Data.Helpers;
using VitalChoice.Data.Repositories;
using VitalChoice.Data.Repositories.Customs;
using VitalChoice.Data.Repositories.Specifics;
using VitalChoice.Data.Services;
using VitalChoice.Data.UnitOfWork;
using VitalChoice.DynamicData.Base;
using VitalChoice.DynamicData.Helpers;
using VitalChoice.Ecommerce.Domain.Entities;
using VitalChoice.Ecommerce.Domain.Entities.Addresses;
using VitalChoice.Ecommerce.Domain.Entities.Affiliates;
using VitalChoice.Ecommerce.Domain.Entities.Base;
using VitalChoice.Ecommerce.Domain.Entities.Customers;
using VitalChoice.Ecommerce.Domain.Entities.GiftCertificates;
using VitalChoice.Ecommerce.Domain.Entities.History;
using VitalChoice.Ecommerce.Domain.Entities.Orders;
using VitalChoice.Ecommerce.Domain.Entities.Payment;
using VitalChoice.Ecommerce.Domain.Entities.Products;
using VitalChoice.Ecommerce.Domain.Exceptions;
using VitalChoice.Ecommerce.Domain.Helpers;
using VitalChoice.Ecommerce.Domain.Transfer;
using VitalChoice.Infrastructure.Domain.Constants;
using VitalChoice.Infrastructure.Domain.Dynamic;
using VitalChoice.Infrastructure.Domain.Entities.Customers;
using VitalChoice.Infrastructure.Domain.Entities.Healthwise;
using VitalChoice.Infrastructure.Domain.Entities.Users;
using VitalChoice.Infrastructure.Domain.Transfer;
using VitalChoice.Infrastructure.Domain.Transfer.Affiliates;
using VitalChoice.Infrastructure.Domain.Transfer.Contexts;
using VitalChoice.Infrastructure.Domain.Transfer.Customers;
using VitalChoice.Infrastructure.Domain.Transfer.Healthwise;
using VitalChoice.Infrastructure.Domain.Transfer.Orders;
using VitalChoice.Infrastructure.UnitOfWork;
using VitalChoice.Interfaces.Services;
using VitalChoice.Interfaces.Services.Affiliates;
using VitalChoice.Interfaces.Services.Customers;
using VitalChoice.Interfaces.Services.Healthwise;
using VitalChoice.Interfaces.Services.Orders;
using VitalChoice.Workflow.Core;

namespace VitalChoice.Business.Services.Healthwise
{
    public class HealthwiseService : IHealthwiseService
    {
        private readonly IEcommerceRepositoryAsync<VHealthwisePeriod> _vHealthwisePeriodRepository;
        private readonly IEcommerceRepositoryAsync<HealthwiseOrder> _healthwiseOrderRepository;
        private readonly OrderRepository _orderRepository;
        private readonly IOrderService _orderService;
        private readonly IEcommerceRepositoryAsync<Customer> _customerRepository;
        private readonly IAppInfrastructureService _appInfrastructureService;
        private readonly ILogger _logger;

        public HealthwiseService(
            IEcommerceRepositoryAsync<VHealthwisePeriod> vHealthwisePeriodRepository,
            IEcommerceRepositoryAsync<HealthwiseOrder> healthwiseOrderRepository,
            OrderRepository orderRepository,
            IOrderService orderService,
            IEcommerceRepositoryAsync<Customer> customerRepository,
            IAppInfrastructureService appInfrastructureService,
            ILoggerProviderExtended loggerProvider)
        {
            _vHealthwisePeriodRepository = vHealthwisePeriodRepository;
            _healthwiseOrderRepository = healthwiseOrderRepository;
            _orderRepository = orderRepository;
            _orderService = orderService;
            _customerRepository = customerRepository;
            _appInfrastructureService = appInfrastructureService;
            _logger = loggerProvider.CreateLoggerDefault();
        }

        public async Task<ICollection<HealthwiseOrder>> GetHealthwiseOrdersAsync(int idPeriod)
        {
            return await _healthwiseOrderRepository.Query(p => p.IdHealthwisePeriod == idPeriod && p.Order.StatusCode != (int)RecordStatusCode.Deleted
                && (p.Order.OrderStatus == OrderStatus.Processed || p.Order.OrderStatus == OrderStatus.Shipped ||
                p.Order.OrderStatus == OrderStatus.Exported)).Include(p => p.Order).SelectAsync(false);
        }

        public async Task<ICollection<VHealthwisePeriod>> GetVHealthwisePeriodsAsync(VHealthwisePeriodFilter filter)
        {
            VHealthwisePeriodQuery conditions = new VHealthwisePeriodQuery().WithDateTo(filter.To).WithDateFrom(filter.From).
                WithAllowPaymentOnly(filter.NotBilledOnly, _appInfrastructureService.Get().AppSettings.HealthwisePeriodMaxItemsCount);

            var toReturn = await _vHealthwisePeriodRepository.Query(conditions).SelectAsync(false);
            return toReturn;
        }

        public async Task<VHealthwisePeriod> GetVHealthwisePeriodAsync(int id)
        {
            var toReturn = (await _vHealthwisePeriodRepository.Query(p=>p.Id==id).SelectAsync(false)).FirstOrDefault();
            return toReturn;
        }

        public async Task<bool> MakeHealthwisePeriodPaymentAsync(int id, decimal amount, DateTime date, bool payAsGC = false)
        {
            using (var uow = new EcommerceUnitOfWork())
            {
                using (var transaction = uow.BeginTransaction())
                {
                    try
                    {
                        var healthwise = (await _vHealthwisePeriodRepository.Query(p => p.Id == id).SelectAsync()).FirstOrDefault();

                        if (healthwise!=null && !healthwise.PaidDate.HasValue && 
                            healthwise.OrdersCount>= _appInfrastructureService.Get().AppSettings.HealthwisePeriodMaxItemsCount)
                        {
                            if(payAsGC)
                            {
                                //TODO: add GC logic later
                            }

                            healthwise.PaidDate = date;
                            healthwise.PaidAmount = amount;

                            await uow.SaveChangesAsync();
                            transaction.Commit();
                        }
                    }
                    catch (Exception e)
                    {
                        transaction.Rollback();
                        throw;
                    }
                }
            }
            return true;
        }

        public async Task<bool> MarkOrdersAsHealthwiseForCustomerIdAsync(int idCustomer)
        {
            var customer = (await _customerRepository.Query(p => p.Id == idCustomer && p.StatusCode != (int)RecordStatusCode.Deleted).SelectAsync(false)).
                FirstOrDefault();
            if(customer==null)
            {
                return false;
            }

            var orders = await _orderRepository.Query(p =>p.IdCustomer==idCustomer && p.StatusCode != (int)RecordStatusCode.Deleted && (p.OrderStatus == OrderStatus.Processed ||
                p.OrderStatus == OrderStatus.Shipped || p.OrderStatus == OrderStatus.Exported)).SelectAsync(false);
            orders = orders.OrderBy(p => p.DateCreated).ToList();
            foreach (var order in orders)
            {
                await _orderService.UpdateHealthwiseOrderAsync(order.Id,true);
            }
            return true;
        }
    }
}