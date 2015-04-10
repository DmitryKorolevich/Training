using System;

namespace VitalChoice.Domain.Exceptions
{
    public class AppValidationException : Exception
    {
        public string Field { get; private set; }

        public AppValidationException(string field,string messageKey) : base(messageKey)
        {
            this.Field = field;
        }

        public AppValidationException(string messageKey) : base(messageKey)
        {
            this.Field = String.Empty;
        }
    }
}