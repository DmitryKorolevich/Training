using System;
using System.Runtime.Serialization;

namespace VitalChoice.Validation.Exceptions
{
    public class NotAgencyUserException : ApiException
    {
        public NotAgencyUserException() {}

        public NotAgencyUserException(string messageKey)
            : base(messageKey) {}
    }
}