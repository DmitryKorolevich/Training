using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using VitalChoice.Business.Queries.Affiliate;
using VitalChoice.Business.Queries.Product;
using VitalChoice.Business.Services.Dynamic;
using VitalChoice.Data.Helpers;
using VitalChoice.Data.Repositories;
using VitalChoice.Data.Repositories.Customs;
using VitalChoice.Data.Repositories.Specifics;
using VitalChoice.Data.UnitOfWork;
using VitalChoice.Domain.Constants;
using VitalChoice.Domain.Entities;
using VitalChoice.Domain.Entities.eCommerce.Affiliates;
using VitalChoice.Domain.Entities.eCommerce.Base;
using VitalChoice.Domain.Entities.eCommerce.Orders;
using VitalChoice.Domain.Entities.eCommerce.Products;
using VitalChoice.Domain.Entities.Users;
using VitalChoice.Domain.Exceptions;
using VitalChoice.Domain.Transfer.Base;
using VitalChoice.Domain.Transfer.Products;
using VitalChoice.DynamicData.Base;
using VitalChoice.DynamicData.Entities;
using VitalChoice.DynamicData.Interfaces;
using VitalChoice.DynamicData.Validation;
using VitalChoice.Infrastructure.UnitOfWork;
using VitalChoice.Interfaces.Services.Orders;
using VitalChoice.Interfaces.Services.Products;
using VitalChoice.Interfaces.Services.Affiliates;
using VitalChoice.Domain.Transfer.Affiliates;
using VitalChoice.Domain.Mail;
using VitalChoice.Business.Mail;
using VitalChoice.Domain.Entities.eCommerce.History;
using VitalChoice.Interfaces.Services;
using VitalChoice.Data.Services;
using VitalChoice.Domain.Entities.Roles;
using VitalChoice.Interfaces.Services.Users;
using VitalChoice.Domain.Entities.Options;
using Microsoft.Extensions.OptionsModel;
using System.Collections;
using VitalChoice.Domain.Entities.eCommerce.Customers;
using VitalChoice.Business.Queries.Customer;
using VitalChoice.DynamicData.Helpers;
using System.Globalization;

namespace VitalChoice.Business.Services.Affiliates
{
    public class AffiliateService : EcommerceDynamicService<AffiliateDynamic, Affiliate, AffiliateOptionType, AffiliateOptionValue>,
        IAffiliateService
    {
        private readonly IEcommerceRepositoryAsync<VAffiliate> _vAffiliateRepository;
        private readonly IRepositoryAsync<AdminProfile> _adminProfileRepository;
        private readonly IEcommerceRepositoryAsync<VCustomerInAffiliate> _vCustomerInAffiliateRepository;
        private readonly AffiliateOrderPaymentRepository _affiliateOrderPaymentRepository;
        private readonly IEcommerceRepositoryAsync<AffiliatePayment> _affiliatePaymentRepository;
        private readonly INotificationService _notificationService;
        private readonly IAffiliateUserService _affiliateUserService;
        private readonly IEcommerceRepositoryAsync<VCustomer> _vCustomerRepositoryAsync;
        private readonly IEcommerceRepositoryAsync<Customer> _customerRepositoryAsync;
        private readonly IOptions<AppOptions> _appOptions;

        public AffiliateService(IEcommerceRepositoryAsync<VAffiliate> vAffiliateRepository,
            IEcommerceRepositoryAsync<Lookup> lookupRepository,
            IEcommerceRepositoryAsync<Affiliate> affiliateRepository,
            IEcommerceRepositoryAsync<BigStringValue> bigStringValueRepository,
            IEcommerceRepositoryAsync<VCustomerInAffiliate> vCustomerInAffiliateRepository,
            AffiliateOrderPaymentRepository affiliateOrderPaymentRepository,
            IEcommerceRepositoryAsync<AffiliatePayment> affiliatePaymentRepository,
            IEcommerceRepositoryAsync<VCustomer> vCustomerRepositoryAsync,
            IEcommerceRepositoryAsync<Customer> customerRepositoryAsync,
            AffiliateMapper mapper,
            IObjectLogItemExternalService objectLogItemExternalService,
            IEcommerceRepositoryAsync<AffiliateOptionValue> affiliateValueRepositoryAsync,
            IRepositoryAsync<AdminProfile> adminProfileRepository,
            INotificationService notificationService,
            IAffiliateUserService affiliateUserService,
            IOptions<AppOptions> appOptions,
            ILoggerProviderExtended loggerProvider, DynamicExpressionVisitor queryVisitor)
            : base(
                mapper, affiliateRepository, affiliateValueRepositoryAsync,
                bigStringValueRepository, objectLogItemExternalService, loggerProvider, queryVisitor)
        {
            _vAffiliateRepository = vAffiliateRepository;
            _vCustomerInAffiliateRepository = vCustomerInAffiliateRepository;
            _affiliateOrderPaymentRepository = affiliateOrderPaymentRepository;
            _affiliatePaymentRepository = affiliatePaymentRepository;
            _vAffiliateRepository = vAffiliateRepository;
            _adminProfileRepository = adminProfileRepository;
            _notificationService = notificationService;
            _affiliateUserService = affiliateUserService;
            _vCustomerRepositoryAsync = vCustomerRepositoryAsync;
            _customerRepositoryAsync = customerRepositoryAsync;
            _appOptions = appOptions;
        }

        #region Affiliate

        protected override bool LogObjectFullData { get { return true; } }

        public async Task<PagedList<VAffiliate>> GetAffiliatesAsync(VAffiliateFilter filter)
        {
            var conditions = new VAffiliateQuery().NotDeleted().WithId(filter.Id).WithTier(filter.Tier).WithName(filter.Name).
                WithCompany(filter.Company);

            var query = _vAffiliateRepository.Query(conditions);

            Func<IQueryable<VAffiliate>, IOrderedQueryable<VAffiliate>> sortable = x => x.OrderBy(y => y.Name);
            var sortOrder = filter.Sorting.SortOrder;
            switch (filter.Sorting.Path)
            {
                case VAffiliateSortPath.Id:
                    sortable =
                        (x) =>
                            sortOrder == SortOrder.Asc
                                ? x.OrderBy(y => y.Id)
                                : x.OrderByDescending(y => y.Id);
                    break;
                case VAffiliateSortPath.Name:
                    sortable =
                        (x) =>
                            sortOrder == SortOrder.Asc
                                ? x.OrderBy(y => y.Name)
                                : x.OrderByDescending(y => y.Name);
                    break;
                case VAffiliateSortPath.WebSite:
                    sortable =
                        (x) =>
                            sortOrder == SortOrder.Asc
                                ? x.OrderBy(y => y.WebSite)
                                : x.OrderByDescending(y => y.WebSite);
                    break;
                case VAffiliateSortPath.Company:
                    sortable =
                        (x) =>
                            sortOrder == SortOrder.Asc
                                ? x.OrderBy(y => y.Company)
                                : x.OrderByDescending(y => y.Company);
                    break;
                case VAffiliateSortPath.StatusCode:
                    sortable =
                        (x) =>
                            sortOrder == SortOrder.Asc
                                ? x.OrderBy(y => y.StatusCode)
                                : x.OrderByDescending(y => y.StatusCode);
                    break;
                case VAffiliateSortPath.CommissionAll:
                    sortable =
                        (x) =>
                            sortOrder == SortOrder.Asc
                                ? x.OrderBy(y => y.CommissionAll)
                                : x.OrderByDescending(y => y.CommissionAll);
                    break;
                case VAffiliateSortPath.DateEdited:
                    sortable =
                        (x) =>
                            sortOrder == SortOrder.Asc
                                ? x.OrderBy(y => y.DateEdited)
                                : x.OrderByDescending(y => y.DateEdited);
                    break;
                case VAffiliateSortPath.Tier:
                    sortable =
                        (x) =>
                            sortOrder == SortOrder.Asc
                                ? x.OrderBy(y => y.Tier)
                                : x.OrderByDescending(y => y.Tier);
                    break;
                case VAffiliateSortPath.CustomersCount:
                    sortable =
                        (x) =>
                            sortOrder == SortOrder.Asc
                                ? x.OrderBy(y => y.CustomersCount)
                                : x.OrderByDescending(y => y.CustomersCount);
                    break;
            }

            var toReturn = await query.OrderBy(sortable).SelectPageAsync(filter.Paging.PageIndex, filter.Paging.PageItemCount);
            if (toReturn.Items.Any())
            {
                var ids = toReturn.Items.Where(p => p.IdEditedBy.HasValue).Select(p => p.IdEditedBy).ToList();
                var profiles = await _adminProfileRepository.Query(p => ids.Contains(p.Id)).SelectAsync();
                foreach (var item in toReturn.Items)
                {
                    foreach (var profile in profiles)
                    {
                        if (item.IdEditedBy == profile.Id)
                        {
                            item.EditedByAgentId = profile.AgentId;
                        }
                    }
                }
            }

            return toReturn;
        }

        public async Task<bool> SendAffiliateEmailAsync(BasicEmail model)
        {
            await _notificationService.SendBasicEmailAsync(model);
            return true;
        }

        public async Task<AffiliateDynamic> InsertAsync(AffiliateDynamic model, string password)
        {
            //TODO: lock writing DB until we read result
            using (var uow = CreateUnitOfWork())
            {
                var entity = await InsertAsync(model, uow, password);
                int id = entity.Id;
                entity = await SelectEntityFirstAsync(o => o.Id == id);
                await LogItemChanges(new[] { await Mapper.FromEntityAsync(entity) });
                return await Mapper.FromEntityAsync(entity);
            }
        }

        public async Task<AffiliateDynamic> UpdateAsync(AffiliateDynamic model, string password)
        {
            //TODO: lock writing DB until we read result
            using (var uow = CreateUnitOfWork())
            {
                var entity = await UpdateAsync(model, uow, password);
                int id = entity.Id;
                entity = await SelectEntityFirstAsync(o => o.Id == id);
                await LogItemChanges(new[] { await Mapper.FromEntityAsync(entity) });
                return await Mapper.FromEntityAsync(entity);
            }
        }

        protected override async Task<Affiliate> InsertAsync(AffiliateDynamic model, IUnitOfWorkAsync uow)
        {
            return await InsertAsync(model, uow, null);
        }

        protected override async Task<Affiliate> UpdateAsync(AffiliateDynamic model, IUnitOfWorkAsync uow)
        {
            return await UpdateAsync(model, uow, null);
        }

        protected override async Task<bool> DeleteAsync(int id, IUnitOfWorkAsync uow, bool physically)
        {
            var toReturn = await base.DeleteAsync(id, uow, physically);
            if (toReturn)
            {
                var appUser = await _affiliateUserService.GetAsync(id);
                if (appUser != null)
                {
                    await _affiliateUserService.DeleteAsync(appUser);
                }
            }

            return toReturn;
        }

        protected override async Task<List<MessageInfo>> Validate(AffiliateDynamic model)
        {
            var errors = new List<MessageInfo>();

            var itemSameEmail = await ObjectRepository.Query(
                        new AffiliateQuery().NotDeleted().Excluding(model.Id).WithEmail(model.Email)).SelectAsync(false);

            if (itemSameEmail.Any())
            {
                throw new AppValidationException(
                    string.Format(ErrorMessagesLibrary.Data[ErrorMessagesLibrary.Keys.EmailIsTakenAlready], model.Email));
            }

            return errors;
        }

        private async Task<Affiliate> InsertAsync(AffiliateDynamic model, IUnitOfWorkAsync uow, string password)
        {
            var roles = new List<RoleType>() { RoleType.Affiliate };

            var appUser = new ApplicationUser()
            {
                FirstName = model.Name,
                LastName = String.Empty,
                Email = model.Email,
                UserName = model.Email,
                TokenExpirationDate = DateTime.Now.AddDays(_appOptions.Value.ActivationTokenExpirationTermDays),
                IsConfirmed = false,
                ConfirmationToken = Guid.NewGuid(),
                IdUserType = UserType.Affiliate,
                Profile = null,
                Status = UserStatus.NotActive
            };

            var suspendedCustomer = (int)AffiliateStatus.Suspended;

            using (var transaction = uow.BeginTransaction())
            {
                try
                {
                    if (!string.IsNullOrWhiteSpace(password))
                    {
                        appUser.IsConfirmed = true;
                        appUser.Status = model.StatusCode == suspendedCustomer
                            ? UserStatus.Disabled
                            : UserStatus.Active;
                    }
                    else
                    {
                        appUser.Status = model.StatusCode == suspendedCustomer ? UserStatus.Disabled : UserStatus.NotActive;
                    }

                    appUser = await _affiliateUserService.CreateAsync(appUser, roles, false, false, password);

                    model.Id = appUser.Id;

                    var affiliate = await base.InsertAsync(model, uow);

                    if (string.IsNullOrWhiteSpace(password) && model.StatusCode != suspendedCustomer)
                    {
                        await _affiliateUserService.SendActivationAsync(model.Email);
                    }

                    transaction.Commit();

                    return affiliate;
                }
                catch (Exception e)
                {
                    if (appUser.Id > 0)
                    {
                        await _affiliateUserService.DeleteAsync(appUser);
                    }

                    transaction.Rollback();
                    throw;
                }
            }
        }

        private async Task<Affiliate> UpdateAsync(AffiliateDynamic model, IUnitOfWorkAsync uow, string password)
        {
            var appUser = await _affiliateUserService.GetAsync(model.Id);
            if (appUser == null)
            {
                throw new AppValidationException(ErrorMessagesLibrary.Data[ErrorMessagesLibrary.Keys.CantFindLogin]);
            }

            switch (model.StatusCode)
            {
                case (int)AffiliateStatus.Active:
                    appUser.Status = UserStatus.Active;
                    break;
                case (int)AffiliateStatus.NotActive:
                    appUser.Status = UserStatus.NotActive;
                    break;
                case (int)AffiliateStatus.Suspended:
                    appUser.Status = UserStatus.Disabled;
                    break;
                case (int)AffiliateStatus.Deleted:
                    appUser.Status = UserStatus.NotActive;
                    appUser.DeletedDate = DateTime.Now;
                    break;
                case (int)AffiliateStatus.Pending:
                    appUser.Status = UserStatus.Active;
                    break;
            }

            if (!string.IsNullOrWhiteSpace(password))
            {
                appUser.IsConfirmed = true;
				appUser.ConfirmationToken = Guid.Empty;
            }

            appUser.Email = model.Email;
            appUser.UserName = model.Email;

            appUser.FirstName = model.Name;
            appUser.LastName = String.Empty;

            //TODO: Investigate transaction read issues (new transaction allocated with any read on the same connection with overwrite/close current
            //using (var transaction = uow.BeginTransaction())
            //{
            //try
            //{
            var affiliate = await base.UpdateAsync(model, uow);

            //transaction.Commit();

            var roles = new List<RoleType>() { RoleType.Affiliate };

            await _affiliateUserService.UpdateAsync(appUser, roles, password);

            return affiliate;
            //}
            //catch (Exception ex)
            //{
            //	transaction.Rollback();
            //	throw;
            //}
            //}
        }

        public async Task<bool> SelectAnyAsync(int id)
        {
            return await this.ObjectRepository.Query(p => p.Id == id && p.StatusCode != (int)RecordStatusCode.Deleted).SelectAnyAsync();
        }

        #endregion

        #region AffiliatePayments

        public async Task<PagedList<VCustomerInAffiliate>> GetCustomerInAffiliateReport(FilterBase filter)
        {
            Func<IQueryable<VCustomerInAffiliate>, IOrderedQueryable<VCustomerInAffiliate>> sortable = x => x.OrderByDescending(y => y.Count);
            var toReturn = await _vCustomerInAffiliateRepository.Query().OrderBy(sortable).SelectPageAsync(filter.Paging.PageIndex, filter.Paging.PageItemCount);
            return toReturn;
        }

        public async Task<ICollection<AffiliatePayment>> GetAffiliatePayments(int idAffiliate)
        {
            var toReturn = (await _affiliatePaymentRepository.Query(p => p.IdAffiliate == idAffiliate).SelectAsync(false)).ToList();
            return toReturn;
        }

        public async Task<PagedList<AffiliateOrderPayment>> GetAffiliateOrderPayments(AffiliateOrderPaymentFilter filter)
        {
            PagedList<AffiliateOrderPayment> toReturn;
            AffiliateOrderPaymentQuery conditions = new AffiliateOrderPaymentQuery().WithIdAffiliate(filter.IdAffiliate).WithPaymentStatus(filter.Status).
                WithOrderStatus().WithFromDate(filter.From).WithToDate(filter.To);

            Func<IQueryable<AffiliateOrderPayment>, IOrderedQueryable<AffiliateOrderPayment>> sortable = x => x.OrderByDescending(y => y.Order.DateCreated);
            var query = _affiliateOrderPaymentRepository.Query(conditions).Include(p => p.Order).OrderBy(sortable);

            if (filter.Paging != null)
            {
                toReturn = await query.SelectPageAsync(filter.Paging.PageIndex, filter.Paging.PageItemCount);
            }
            else
            {
                var items = (await query.SelectAsync(false)).ToList();
                toReturn = new PagedList<AffiliateOrderPayment>()
                {
                    Items = items,
                    Count = items.Count
                };
            }
            return toReturn;
        }

        public async Task<bool> DeleteAffiliateOrderPayment(int idOrder)
        {
            var dbItem = (await _affiliateOrderPaymentRepository.Query(p => p.Id == idOrder).SelectAsync()).FirstOrDefault();
            if (dbItem != null)
            {
                if(dbItem.Status==AffiliateOrderPaymentStatus.Paid)
                {
                    return false;
                }

                await _affiliateOrderPaymentRepository.DeleteAsync(dbItem);
            }
            return true;
        }

        public async Task<AffiliateOrderPayment> UpdateAffiliateOrderPayment(AffiliateOrderPayment item)
        {
            var dbItem = (await _affiliateOrderPaymentRepository.Query(p=>p.Id==item.Id).SelectAsync()).FirstOrDefault();
            if(dbItem==null)
            {
                dbItem = new AffiliateOrderPayment();
                dbItem.Status = AffiliateOrderPaymentStatus.NotPaid;
                dbItem.IdAffiliate = item.IdAffiliate;
                dbItem.Id = item.Id;
                dbItem.Amount = item.Amount;
                dbItem.NewCustomerOrder = item.NewCustomerOrder;

                await _affiliateOrderPaymentRepository.InsertAsync(dbItem);
            }
            else
            {
                if(dbItem.Status == AffiliateOrderPaymentStatus.NotPaid)
                {
                    dbItem.Amount = item.Amount;

                    await _affiliateOrderPaymentRepository.UpdateAsync(dbItem);
                }
            }

            return dbItem;
        }

        public async Task<AffiliatesSummaryModel> GetAffiliatesSummary()
        {
            AffiliatesSummaryModel toReturn = new AffiliatesSummaryModel();
            AffiliateQuery conditions = new AffiliateQuery().NotDeleted();
            toReturn.AllAffiliates = await ObjectRepository.Query(conditions).SelectCountAsync();
            toReturn.EngagedAffiliates = await _affiliateOrderPaymentRepository.GetEngangedAffiliatesCount();
            CustomerQuery customerCoditions = new CustomerQuery().NotDeleted().WithAffiliate();
            toReturn.AffiliateCustomers = await _customerRepositoryAsync.Query(customerCoditions).SelectCountAsync();

            return toReturn;
        }

        public async Task<ICollection<AffiliatesSummaryReportItemModel>> GetAffiliatesSummaryReportItemsForMonths(DateTime lastMonthStartDay, int monthCount)
        {
            List<AffiliatesSummaryReportItemModel> toReturn = new List<AffiliatesSummaryReportItemModel>();
            while(monthCount>0)
            {
                var spItems = await _affiliateOrderPaymentRepository.GetAffiliatesSummaryReport(lastMonthStartDay, lastMonthStartDay.AddMonths(1));

                var newStatistic = spItems.FirstOrDefault(p => p.IdType == 1);
                var repeatStatistic = spItems.FirstOrDefault(p => p.IdType == 2);
                AffiliatesSummaryReportItemModel item = new AffiliatesSummaryReportItemModel();
                item.Month = lastMonthStartDay.ToString("MMMM",CultureInfo.InvariantCulture);
                if(newStatistic!=null)
                {
                    item.NewTransactions = newStatistic.Count;
                    item.NewSales = newStatistic.Sum;
                }
                if (repeatStatistic != null)
                {
                    item.RepeatTransactions = repeatStatistic.Count;
                    item.RepeatSales = repeatStatistic.Sum;
                    item.TotalTransactions = item.NewTransactions + item.RepeatTransactions;
                    item.TotalSales = item.NewSales + item.RepeatSales;
                    if (item.TotalTransactions != 0)
                    {
                        item.NewTransactionsPercent =Math.Round((((decimal)item.NewTransactions) / item.TotalTransactions)*100,2);
                    }
                    if (item.TotalSales != 0)
                    {
                        item.NewSalesPercent = Math.Round((((decimal)item.NewSales) / item.TotalSales)*100,2);
                    }
                }
                toReturn.Add(item);

                lastMonthStartDay = lastMonthStartDay.AddMonths(-1);
                monthCount--;
            }

            return toReturn;
        }

        #endregion
    }
}