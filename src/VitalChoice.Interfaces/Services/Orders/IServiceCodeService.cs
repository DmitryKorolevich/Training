using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using VitalChoice.Data.Helpers;
using VitalChoice.DynamicData.Interfaces;
using VitalChoice.Ecommerce.Domain.Entities.GiftCertificates;
using VitalChoice.Ecommerce.Domain.Entities.Orders;
using VitalChoice.Ecommerce.Domain.Exceptions;
using VitalChoice.Ecommerce.Domain.Transfer;
using VitalChoice.Infrastructure.Domain.Dynamic;
using VitalChoice.Infrastructure.Domain.Entities.Orders;
using VitalChoice.Infrastructure.Domain.Transfer.Affiliates;
using VitalChoice.Infrastructure.Domain.Transfer.Contexts;
using VitalChoice.Infrastructure.Domain.Transfer.GiftCertificates;
using VitalChoice.Infrastructure.Domain.Transfer.Orders;

namespace VitalChoice.Interfaces.Services.Orders
{
    public interface IServiceCodeService
    {
        Task<ServiceCodesReport> GetServiceCodesReportAsync(ServiceCodesReportFilter filter);

        Task<PagedList<ServiceCodeRefundItem>> GetServiceCodeRefundItemsAsync(ServiceCodeItemsFilter filter);

        Task<PagedList<ServiceCodeReshipItem>> GetServiceCodeReshipItemsAsync(ServiceCodeItemsFilter filter);

        Task<bool> AssignServiceCodeForRefundsAsync(IEnumerable<int> ids, int serviceCode);

        Task<bool> AssignServiceCodeForReshipsAsync(IEnumerable<int> ids, int serviceCode);
    }
}
