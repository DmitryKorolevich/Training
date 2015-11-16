namespace VitalChoice.Ecommerce.Domain.Exceptions
{
    public class AffiliatePendingException : AppValidationException
    {
        public AffiliatePendingException(string message) : base(message)
        {
        }
    }
}