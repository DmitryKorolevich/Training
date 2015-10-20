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

namespace VitalChoice.Business.Services.Affiliates
{
    public class AffiliateService : EcommerceDynamicObjectService<AffiliateDynamic, Affiliate, AffiliateOptionType, AffiliateOptionValue>, IAffiliateService
    {
        private readonly IEcommerceRepositoryAsync<VAffiliate> _vAffiliateRepository;
        private readonly IRepositoryAsync<AdminProfile> _adminProfileRepository;
        private readonly INotificationService _notificationService;

        public AffiliateService(IEcommerceRepositoryAsync<VAffiliate> vAffiliateRepository,
            IEcommerceRepositoryAsync<AffiliateOptionType> affiliateOptionTypeRepository,
            IEcommerceRepositoryAsync<Lookup> lookupRepository, IEcommerceRepositoryAsync<Affiliate> affiliateRepository,
            IEcommerceRepositoryAsync<BigStringValue> bigStringValueRepository,
            AffiliateMapper mapper,
            IObjectLogItemExternalService objectLogItemExternalService,
            IEcommerceRepositoryAsync<AffiliateOptionValue> affiliateValueRepositoryAsync,
            IRepositoryAsync<AdminProfile> adminProfileRepository, INotificationService notificationService,
            ILoggerProviderExtended loggerProvider)
            : base(
                mapper, affiliateRepository, affiliateOptionTypeRepository, affiliateValueRepositoryAsync,
                bigStringValueRepository, objectLogItemExternalService, loggerProvider)
        {
            _vAffiliateRepository = vAffiliateRepository;
            _adminProfileRepository = adminProfileRepository;
            _notificationService = notificationService;
        }

        protected override bool LogObject { get { return false; } }

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
            }

            var toReturn = await query.OrderBy(sortable).SelectPageAsync(filter.Paging.PageIndex, filter.Paging.PageItemCount);
            if (toReturn.Items.Any())
            {
                var ids = toReturn.Items.Where(p=>p.IdEditedBy.HasValue).Select(p => p.IdEditedBy).ToList();
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
    }
}