using System;

namespace VitalChoice.Domain.Exceptions
{
    public class AppValidationException : Exception
    {
        public string Field { get; private set; }

        public AppValidationException(string field,string message) : base(message)
        {
            this.Field = field;
        }

        public AppValidationException(string message) : base(message)
        {
            this.Field = String.Empty;
        }
    }
}