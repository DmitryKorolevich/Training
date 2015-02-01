using System;
using System.Collections.Generic;
using System.Net;

namespace VitalChoice.Validation.Exceptions
{
    public class ApiValidationException: ApiException
    {
        public ApiValidationException(string messageKey, IEnumerable<object> args = null)
            : base(messageKey, HttpStatusCode.OK, args) { }
    }
}