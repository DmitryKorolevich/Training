using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using VitalChoice.Business.Mailings;
using VitalChoice.Business.Queries.Healthwise;
using VitalChoice.Business.Repositories;
using VitalChoice.Data.Repositories.Specifics;
using VitalChoice.Data.Services;
using VitalChoice.Data.Transaction;
using VitalChoice.DynamicData.Base;
using VitalChoice.DynamicData.Helpers;
using VitalChoice.Ecommerce.Domain.Entities;
using VitalChoice.Ecommerce.Domain.Entities.Customers;
using VitalChoice.Ecommerce.Domain.Entities.GiftCertificates;
using VitalChoice.Ecommerce.Domain.Entities.Healthwise;
using VitalChoice.Ecommerce.Domain.Entities.Orders;
using VitalChoice.Ecommerce.Domain.Exceptions;
using VitalChoice.Ecommerce.Domain.Mail;
using VitalChoice.Ecommerce.Domain.Options;
using VitalChoice.Ecommerce.Domain.Transfer;
using VitalChoice.Infrastructure.Context;
using VitalChoice.Infrastructure.Domain.Constants;
using VitalChoice.Infrastructure.Domain.Dynamic;
using VitalChoice.Infrastructure.Domain.Entities.Customers;
using VitalChoice.Infrastructure.Domain.Entities.Healthwise;
using VitalChoice.Infrastructure.Domain.Entities.Settings;
using VitalChoice.Infrastructure.Domain.Transfer;
using VitalChoice.Infrastructure.Domain.Transfer.Healthwise;
using VitalChoice.Infrastructure.UOW;
using VitalChoice.Interfaces.Services;
using VitalChoice.Interfaces.Services.Customers;
using VitalChoice.Interfaces.Services.Healthwise;
using VitalChoice.Interfaces.Services.Orders;
using VitalChoice.Interfaces.Services.Products;

namespace VitalChoice.Business.Services.Healthwise
{
    public class HealthwiseService : IHealthwiseService
    {
        private readonly IEcommerceRepositoryAsync<VHealthwisePeriod> _vHealthwisePeriodRepository;
        private readonly IEcommerceRepositoryAsync<HealthwiseOrder> _healthwiseOrderRepository;
        private readonly IEcommerceRepositoryAsync<HealthwisePeriod> _healthwisePeriodRepository;
        private readonly IOrderService _orderService;
        private readonly IEcommerceRepositoryAsync<Customer> _customerRepository;
        private readonly IGcService _gcService;
        private readonly INotificationService _notificationService;
        private readonly ITransactionAccessor<EcommerceContext> _transactionAccessor;
        private readonly DbContextOptions<EcommerceContext> _eccomerceContextOptions;
        private readonly AppSettings _appSettings;
        private readonly ILogger _logger;
        private readonly IOptions<AppOptionsBase> _options;
        private readonly ICustomerService _customerService;

        public HealthwiseService(
            IEcommerceRepositoryAsync<VHealthwisePeriod> vHealthwisePeriodRepository,
            IEcommerceRepositoryAsync<HealthwiseOrder> healthwiseOrderRepository,
            IEcommerceRepositoryAsync<HealthwisePeriod> healthwisePeriodRepository,
            IOrderService orderService,
            IEcommerceRepositoryAsync<Customer> customerRepository,
            IGcService gcService,
            INotificationService notificationService,
            ITransactionAccessor<EcommerceContext> transactionAccessor,
            ILoggerFactory loggerProvider, 
            DbContextOptions<EcommerceContext> eccomerceContextOptions, 
            AppSettings appSettings,
            IOptions<AppOptionsBase> options,
            ICustomerService customerService)
        {
            _vHealthwisePeriodRepository = vHealthwisePeriodRepository;
            _healthwiseOrderRepository = healthwiseOrderRepository;
            _healthwisePeriodRepository = healthwisePeriodRepository;
            _orderService = orderService;
            _customerRepository = customerRepository;
            _gcService = gcService;
            _notificationService = notificationService;
            _transactionAccessor = transactionAccessor;
            _eccomerceContextOptions = eccomerceContextOptions;
            _appSettings = appSettings;
            _options = options;
            _logger = loggerProvider.CreateLogger<HealthwiseService>();
            _customerService = customerService;
        }

        public async Task<ICollection<HealthwiseOrder>> GetHealthwiseOrdersAsync(int idPeriod)
        {
            Func<IQueryable<HealthwiseOrder>, IOrderedQueryable<HealthwiseOrder>> sortable =
                x => x.OrderByDescending(y => y.Order.DateCreated);
            return
                await
                    _healthwiseOrderRepository.Query(
                        p => p.IdHealthwisePeriod == idPeriod && p.Order.StatusCode != (int) RecordStatusCode.Deleted
                             && (p.Order.OrderStatus == OrderStatus.Processed || p.Order.OrderStatus == OrderStatus.Shipped ||
                                 p.Order.OrderStatus == OrderStatus.Exported ||
                                 ((p.Order.POrderStatus == OrderStatus.Processed || p.Order.POrderStatus == OrderStatus.Shipped ||
                                   p.Order.POrderStatus == OrderStatus.Exported) &&
                                  (p.Order.NPOrderStatus == OrderStatus.Processed || p.Order.NPOrderStatus == OrderStatus.Shipped ||
                                   p.Order.NPOrderStatus == OrderStatus.Exported))
                                 )).Include(p => p.Order).OrderBy(sortable).SelectAsync(false);
        }

        public async Task<ICollection<VHealthwisePeriod>> GetVHealthwisePeriodsAsync(VHealthwisePeriodFilter filter)
        {
            VHealthwisePeriodQuery conditions = new VHealthwisePeriodQuery()
                .WithCustomerId(filter.IdCustomer)
                .WithCustomerFirstName(filter.CustomerFirstName)
                .WithCustomerLastName(filter.CustomerLastName)
                .WithDateTo(filter.To)
                .WithDateFrom(filter.From)
                .WithAllowPaymentOnly(filter.NotBilledOnly, _appSettings.HealthwisePeriodMaxItemsCount)
                .WithNotPaid(filter.NotPaid);

            var toReturn = await _vHealthwisePeriodRepository.Query(conditions).SelectAsync(false);
            return toReturn;
        }

        public async Task<VHealthwisePeriod> GetVHealthwisePeriodAsync(int id)
        {
            var toReturn = (await _vHealthwisePeriodRepository.Query(p => p.Id == id).SelectFirstOrDefaultAsync(false));
            return toReturn;
        }

        public async Task<bool> DeleteHealthwisePeriod(int id)
        {
            var orders = await GetHealthwiseOrdersAsync(id);
            if (orders.Count > 0)
            {
                throw new AppValidationException("Can't delete period with assigned orders.");
            }

            await _healthwisePeriodRepository.DeleteAsync(id);

            return true;
        }

        public async Task<bool> MakeHealthwisePeriodPaymentAsync(int id, decimal amount, DateTime date, bool payAsGC = false,
            int? userId = null)
        {
            VHealthwisePeriod paidHealthwise = null;
            string paidGCCode = null;
            GiftAdminNotificationEmail notificationModel = new GiftAdminNotificationEmail();
            using (var uow = new EcommerceUnitOfWork(_eccomerceContextOptions, _options))
            {
                using (var transaction = uow.BeginTransaction())
                {
                    try
                    {
                        var vHealthwisePeriodRepository = uow.RepositoryAsync<VHealthwisePeriod>();
                        var giftCertificateRepository = uow.RepositoryAsync<GiftCertificate>();
                        var healthwise = await vHealthwisePeriodRepository.Query(p => p.Id == id).SelectFirstOrDefaultAsync(true);
                        
                        if (healthwise != null && !healthwise.PaidDate.HasValue &&
                            healthwise.OrdersCount >= _appSettings.HealthwisePeriodMaxItemsCount)
                        {
                            if (payAsGC)
                            {
                                GiftCertificate gc = new GiftCertificate();
                                gc.Created = DateTime.Now;
                                gc.GCType = GCType.ManualGC;
                                gc.StatusCode = RecordStatusCode.Active;
                                gc.Email = healthwise.CustomerEmail;
                                gc.FirstName = healthwise.CustomerFirstName;
                                gc.LastName = healthwise.CustomerLastName;
                                gc.Balance = amount;
                                gc.Code = await _gcService.GenerateGCCode();
                                gc.UserId = userId;
                                gc.IdEditedBy = userId;

                                giftCertificateRepository.Insert(gc);
                                notificationModel.Email = gc.Email;
                                notificationModel.Recipient =$"{gc.FirstName} {gc.LastName}";
                                notificationModel.Gifts =new List<GiftEmailModel>()
                                {
                                    new GiftEmailModel()
                                    {
                                        Amount = gc.Balance,
                                        Code = gc.Code
                                    }
                                };

                                paidGCCode = gc.Code;
                            }

                            healthwise.PaidDate = date;
                            healthwise.PaidAmount = amount;

                            await uow.SaveChangesAsync();
                            transaction.Commit();

                            paidHealthwise = healthwise;
                        }
                    }
                    catch
                    {
                        transaction.Rollback();
                        throw;
                    }
                }
            }
            if (paidHealthwise!=null)
            {
                var customer = await _customerService.SelectAsync(paidHealthwise.IdCustomer);

                var note = new CustomerNoteDynamic();
                note.IdAddedBy = userId;
                if (payAsGC)
                {
                    note.Note = $"HealthWise GC Payment - {paidGCCode} - ${paidHealthwise.PaidAmount}";
                }
                else
                {
                    note.Note = $"HealthWise GC Payment - ${paidHealthwise.PaidAmount}";
                }
                note.Data.Priority = (int)CustomerNotePriority.NormalPriority;
                customer.CustomerNotes.Add(note);
                customer.IdEditedBy = userId;

                await _customerService.UpdateAsync(customer);
            }
            if (payAsGC)
            {
                await _notificationService.SendGiftAdminNotificationEmailAsync(notificationModel.Email, notificationModel);
            }
            return true;
        }

        public async Task<bool> MarkOrdersAsHealthwiseForCustomerIdAsync(int idCustomer)
        {
            var customer =
                await
                    _customerRepository.Query(p => p.Id == idCustomer && p.StatusCode != (int) RecordStatusCode.Deleted)
                        .SelectFirstOrDefaultAsync(false);
            if (customer == null)
            {
                return false;
            }

            using (var uow = _transactionAccessor.CreateUnitOfWork())
            {
                await _orderService.MarkHealthwiseCustomerAsync(uow, customer.Id);
                await uow.SaveChangesAsync();
            }

            return true;
        }

        public async Task<ICollection<VHealthwisePeriod>> GetVHealthwisePeriodsForMovementAsync(int idPeriod, int count)
        {
            ICollection<VHealthwisePeriod> toReturn = new List<VHealthwisePeriod>();

            var period = await _vHealthwisePeriodRepository.Query(p => p.Id == idPeriod).SelectFirstOrDefaultAsync(false);

            if (period != null && count != 0)
            {
                int maxCount = _appSettings.HealthwisePeriodMaxItemsCount;
                VHealthwisePeriodQuery conditions = new VHealthwisePeriodQuery().WithCustomerId(period.IdCustomer).WithNotPaid(true);
                var items = await _vHealthwisePeriodRepository.Query(conditions).SelectAsync(false);
                foreach (var item in items)
                {
                    if (item.OrdersCount + count <= maxCount && item.Id != period.Id)
                    {
                        toReturn.Add(item);
                    }
                }
            }

            return toReturn;
        }

        public async Task<bool> MoveHealthwiseOrdersAsync(int idPeriod, ICollection<int> ids)
        {
            var period = await _vHealthwisePeriodRepository.Query(p => p.Id == idPeriod).SelectFirstOrDefaultAsync(false);
            if (period != null && !period.PaidDate.HasValue)
            {
                using (var uow = new EcommerceUnitOfWork(_eccomerceContextOptions, _options))
                {
                    using (var transaction = uow.BeginTransaction())
                    {
                        try
                        {
                            int maxCount = _appSettings.HealthwisePeriodMaxItemsCount;
                            var healthwiseOrderRepository = uow.RepositoryAsync<HealthwiseOrder>();
                            var orders =
                                await healthwiseOrderRepository.Query(p => ids.Contains(p.Id)).Include(p => p.Order).SelectAsync(true);

                            if (period.OrdersCount + orders.Count > maxCount)
                            {
                                return false;
                            }
                            bool notValidCustomer = false;
                            foreach (var order in orders)
                            {
                                if (order.Order.IdCustomer != period.IdCustomer)
                                {
                                    notValidCustomer = true;
                                    break;
                                }
                            }
                            if (notValidCustomer)
                            {
                                return false;
                            }

                            foreach (var order in orders)
                            {
                                order.IdHealthwisePeriod = period.Id;
                            }

                            await uow.SaveChangesAsync();
                            transaction.Commit();
                        }
                        catch
                        {
                            transaction.Rollback();
                            throw;
                        }
                    }
                }
            }
            return true;
        }

        public async Task<bool> UpdatePeriodDatesAsync(int id, DateTime startDate)
        {
            var item = await _healthwisePeriodRepository.Query(p=>p.Id== id).SelectFirstOrDefaultAsync(true);
            if (item != null)
            {
                item.StartDate = startDate;
                item.EndDate = item.StartDate.AddYears(1);
                await _healthwisePeriodRepository.UpdateAsync(item);

                return true;
            }

            return false;
        }

        public async Task<HealthwisePeriod> AddPeriodAsync(int idCustomer)
        {
            HealthwisePeriod period = new HealthwisePeriod();
            period.IdCustomer = idCustomer;
            period.StartDate = DateTime.Now;
            period.EndDate = period.StartDate.AddYears(1);
            await _healthwisePeriodRepository.InsertAsync(period);
            return period;
        }
    }
}