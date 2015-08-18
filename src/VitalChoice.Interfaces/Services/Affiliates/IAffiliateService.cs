using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VitalChoice.Domain.Entities.eCommerce.Affiliates;
using VitalChoice.Domain.Transfer.Base;
using VitalChoice.DynamicData.Entities;
using VitalChoice.DynamicData.Interfaces;

namespace VitalChoice.Interfaces.Services.Affiliates
{
    public interface IAffiliateService : IDynamicObjectServiceAsync<AffiliateDynamic, Affiliate>
    {
        Task<PagedList<VAffiliate>> GetAffiliatesAsync(FilterBase filter);
    }
}
