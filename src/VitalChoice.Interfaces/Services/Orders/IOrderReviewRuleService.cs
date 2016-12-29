using System.Collections.Generic;
using System.Threading.Tasks;
using VitalChoice.Data.UOW;
using VitalChoice.DynamicData.Interfaces;
using VitalChoice.Ecommerce.Domain.Entities.Orders;
using VitalChoice.Ecommerce.Domain.Transfer;
using VitalChoice.Infrastructure.Domain.Avatax;
using VitalChoice.Infrastructure.Domain.Dynamic;
using VitalChoice.Infrastructure.Domain.Entities.Orders;
using VitalChoice.Infrastructure.Domain.Transfer;
using VitalChoice.Infrastructure.Domain.Transfer.Affiliates;
using VitalChoice.Infrastructure.Domain.Transfer.Contexts;
using VitalChoice.Infrastructure.Domain.Transfer.Orders;

namespace VitalChoice.Interfaces.Services.Orders
{
    public interface IOrderReviewRuleService : IDynamicServiceAsync<OrderReviewRuleDynamic, OrderReviewRule>
    {
        Task<PagedList<OrderReviewRuleDynamic>> GetShortOrderReviewRulesAsync(FilterBase filter);
    }
}
