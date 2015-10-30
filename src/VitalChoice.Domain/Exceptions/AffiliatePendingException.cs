using System;
using System.Collections.Generic;

namespace VitalChoice.Domain.Exceptions
{
    public class AffiliatePendingException : AppValidationException
    {
        public AffiliatePendingException(string message) : base(message)
        {
        }
    }
}