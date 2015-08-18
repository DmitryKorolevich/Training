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
using VitalChoice.Interfaces.Services.Order;
using VitalChoice.Interfaces.Services.Products;
using VitalChoice.Interfaces.Services.Affiliates;

namespace VitalChoice.Business.Services.Affiliates
{
    public class AffiliateService : EcommerceDynamicObjectService<AffiliateDynamic, Affiliate, AffiliateOptionType, AffiliateOptionValue>, IAffiliateService
    {
        private readonly IEcommerceRepositoryAsync<VAffiliate> _vAffiliateRepository;
        private readonly IRepositoryAsync<AdminProfile> _adminProfileRepository;

        public AffiliateService(IEcommerceRepositoryAsync<VAffiliate> vAffiliateRepository,
            IEcommerceRepositoryAsync<AffiliateOptionType> affiliateOptionTypeRepository,
            IEcommerceRepositoryAsync<Lookup> lookupRepository, IEcommerceRepositoryAsync<Affiliate> affiliateRepository,
            IEcommerceRepositoryAsync<BigStringValue> bigStringValueRepository,
            AffiliateMapper mapper,
            IEcommerceRepositoryAsync<AffiliateOptionValue> affiliateValueRepositoryAsync,
            IRepositoryAsync<AdminProfile> adminProfileRepository)
            : base(
                mapper, affiliateRepository, affiliateOptionTypeRepository, affiliateValueRepositoryAsync,
                bigStringValueRepository)
        {
            _vAffiliateRepository = vAffiliateRepository;
            _adminProfileRepository = adminProfileRepository;
        }

        public async Task<PagedList<VAffiliate>> GetAffiliatesAsync(FilterBase filter)
        {
            var conditions = new VAffiliateQuery().NotDeleted();

            var query = _vAffiliateRepository.Query(conditions);

            Func<IQueryable<VAffiliate>, IOrderedQueryable<VAffiliate>> sortable = x => x.OrderByDescending(y => y.Id);
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
            }

            var toReturn = await query.OrderBy(sortable).SelectPageAsync(filter.Paging.PageIndex, filter.Paging.PageItemCount);

            return toReturn;
        }
    }
}