namespace VitalChoice.Ecommerce.Domain.Exceptions
{
    public class WholesalePendingException : AppValidationException
    {
        public WholesalePendingException(string message) : base(message)
        {
        }
    }
}