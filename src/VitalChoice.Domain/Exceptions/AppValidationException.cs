using System;

namespace VitalChoice.Domain.Exceptions
{
    public class AppValidationException : Exception
    {
        public AppValidationException(string messageKey) : base(messageKey)
        {
        }
    }
}