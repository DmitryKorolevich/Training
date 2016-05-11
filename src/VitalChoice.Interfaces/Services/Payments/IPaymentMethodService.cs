﻿using System.Collections.Generic;
using System.Threading.Tasks;
using VitalChoice.Ecommerce.Domain.Entities.Payment;
using VitalChoice.Ecommerce.Domain.Exceptions;
using VitalChoice.Infrastructure.Domain.Dynamic;
using VitalChoice.Infrastructure.Domain.Transfer.PaymentMethod;

namespace VitalChoice.Interfaces.Services.Payments
{
    public interface IPaymentMethodService
    {
	    Task<IList<ExtendedPaymentMethod>> GetApprovedPaymentMethodsAsync();

	    Task SetStateAsync(IList<PaymentMethodsAvailability> paymentMethodsAvailability, int currentUserId);

	    Task<PaymentMethod> GetStorefrontDefaultPaymentMethod();

        Task<List<MessageInfo>> AuthorizeCreditCard(PaymentMethodDynamic paymentMethod);

        bool ValidateCreditCard(PaymentMethodDynamic paymentMethod);
    }
}
