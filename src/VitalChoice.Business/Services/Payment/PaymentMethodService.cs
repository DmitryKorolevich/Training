using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNet.Hosting;
using Microsoft.Framework.Logging;
using VitalChoice.Business.Queries.Payment;
using VitalChoice.Business.Queries.User;
using VitalChoice.Data.Repositories;
using VitalChoice.Data.Repositories.Specifics;
using VitalChoice.Data.Transaction;
using VitalChoice.Domain.Entities.eCommerce.Payment;
using VitalChoice.Domain.Entities.Users;
using VitalChoice.Domain.Transfer.PaymentMethod;
using VitalChoice.Infrastructure.Context;
using VitalChoice.Infrastructure.UnitOfWork;
using VitalChoice.Interfaces.Services.Payment;

namespace VitalChoice.Business.Services.Payment
{
	public class PaymentMethodService : IPaymentMethodService
	{
		private readonly IEcommerceRepositoryAsync<PaymentMethod> _paymentMethodRepository;
		private readonly IRepositoryAsync<AdminProfile> _adminProfileRepository;
		private readonly EcommerceContext _context;
		private readonly IEcommerceRepositoryAsync<PaymentMethodToCustomerType> _paymentMethodToCustomerTypeRepository;
		private readonly IHttpContextAccessor _contextAccessor;
		private readonly ILogger _logger;

		public PaymentMethodService(IEcommerceRepositoryAsync<PaymentMethod> paymentMethodRepository,
			IHttpContextAccessor contextAccessor, IRepositoryAsync<AdminProfile> adminProfileRepository, EcommerceContext context, IEcommerceRepositoryAsync<PaymentMethodToCustomerType> paymentMethodToCustomerTypeRepository)
		{
			_paymentMethodRepository = paymentMethodRepository;
			_contextAccessor = contextAccessor;
			_adminProfileRepository = adminProfileRepository;
			_context = context;
			_paymentMethodToCustomerTypeRepository = paymentMethodToCustomerTypeRepository;
			_logger = LoggerService.GetDefault();
		}

		public async Task<IList<ExtendedPaymentMethod>> GetApprovedPaymentMethodsAsync()
		{
			var condition = new PaymentMethodQuery().NotDeleted();

			var query = _paymentMethodRepository.Query(condition).OrderBy(x => x.OrderBy(y => y.Name));

			var paymentMethods = await query.Include(x => x.CustomerTypes).SelectAsync(false);

			var adminProfileCondition =
				new AdminProfileQuery().IdInRange(
					paymentMethods.Where(x => x.IdEditedBy.HasValue).Select(x => x.IdEditedBy.Value).ToList());

			var adminProfiles = await _adminProfileRepository.Query(adminProfileCondition).SelectAsync(false);

			var result = paymentMethods.Select(paymentMethod => new ExtendedPaymentMethod
			{
				AdminProfile = adminProfiles.SingleOrDefault(x => x.Id == paymentMethod.IdEditedBy),
				IdEditedBy = paymentMethod.IdEditedBy,
				CustomerTypes = paymentMethod.CustomerTypes,
				DateCreated = paymentMethod.DateCreated,
				DateEdited = paymentMethod.DateEdited,
				EditedBy = paymentMethod.EditedBy,
				Id = paymentMethod.Id,
				Name = paymentMethod.Name,
				Order = paymentMethod.Order,
				StatusCode = paymentMethod.StatusCode
			}).ToList();

			return result;
		}

		public async Task SetStateAsync(IList<PaymentMethodsAvailability> paymentMethodsAvailability)
		{
			var currentUserId = Convert.ToInt32(_contextAccessor.HttpContext.User.GetUserId());

			var paymentMethods =
				await
					_paymentMethodRepository.Query(new PaymentMethodQuery().NotDeleted())
						.Include(x => x.CustomerTypes)
						.SelectAsync(false);

			using (var transaction = new TransactionManager(_context).BeginTransaction())
			{
				try
				{
					foreach (var paymentMethod in paymentMethods)
					{
						var needToUpdate = false;

						var assignment = paymentMethodsAvailability.FirstOrDefault(x => x.Id == paymentMethod.Id);
						if (assignment?.CustomerTypes != null && assignment.CustomerTypes.Any())
						{
							if (!paymentMethod.CustomerTypes.Any() || paymentMethod.CustomerTypes.Count != assignment.CustomerTypes.Count || !paymentMethod.CustomerTypes.All(x => assignment.CustomerTypes.Contains(x.IdCustomerType)))
							{
								await _paymentMethodToCustomerTypeRepository.DeleteAllAsync(paymentMethod.CustomerTypes);
                                paymentMethod.CustomerTypes.Clear();
								foreach (var customerType in assignment.CustomerTypes)
								{
									paymentMethod.CustomerTypes.Add(new PaymentMethodToCustomerType()
									{
										IdCustomerType = customerType,
										IdPaymentMethod = paymentMethod.Id,
									});
								}
								if (assignment.CustomerTypes.Any())
								{
									await _paymentMethodToCustomerTypeRepository.InsertRangeAsync(paymentMethod.CustomerTypes);
								}
								needToUpdate = true;
							}
						}
						else
						{
							if (paymentMethod.CustomerTypes != null && paymentMethod.CustomerTypes.Any())
							{
								await _paymentMethodToCustomerTypeRepository.DeleteAllAsync(paymentMethod.CustomerTypes);
								needToUpdate = true;
							}
						}

						if (needToUpdate)
						{
							paymentMethod.DateEdited = DateTime.Now;
							paymentMethod.IdEditedBy = currentUserId;
							await _paymentMethodRepository.UpdateAsync(paymentMethod);
						}
					}

					transaction.Commit();
				}
				catch (Exception)
				{
					transaction.Rollback();
					throw;
				}
			}
		}
	}
}

