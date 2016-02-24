using VitalChoice.Ecommerce.Domain.Exceptions;
using VitalChoice.Infrastructure.Domain.Constants;

namespace VitalChoice.Infrastructure.Domain.Exceptions
{
    public class CustomerSuspendException : AppValidationException
    {
        public CustomerSuspendException() : base(
                    ErrorMessagesLibrary.Data[ErrorMessagesLibrary.Keys.SuspendedCustomer])
        {
        }
    }
}