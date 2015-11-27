using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using VitalChoice.DynamicData.Interfaces;
using VitalChoice.Ecommerce.Domain.Entities.Orders;
using VitalChoice.Ecommerce.Domain.Transfer;
using VitalChoice.Infrastructure.Domain.Dynamic;
using VitalChoice.Infrastructure.Domain.Entities.Healthwise;
using VitalChoice.Infrastructure.Domain.Transfer.Affiliates;
using VitalChoice.Infrastructure.Domain.Transfer.Contexts;
using VitalChoice.Infrastructure.Domain.Transfer.Healthwise;
using VitalChoice.Infrastructure.Domain.Transfer.Orders;

namespace VitalChoice.Interfaces.Services.Healthwise
{
	public interface IHealthwiseService
	{
        Task<ICollection<HealthwiseOrder>> GetHealthwiseOrdersAsync(int idPeriod);

        Task<ICollection<VHealthwisePeriod>> GetVHealthwisePeriodsAsync(VHealthwisePeriodFilter filter);

        Task<VHealthwisePeriod> GetVHealthwisePeriodAsync(int id);

        Task<bool> MakeHealthwisePeriodPaymentAsync(int id, decimal amount, DateTime date, bool payAsGC = false);

        Task<bool> MarkOrdersAsHealthwiseForCustomerIdAsync(int idCustomer);
    }
}
