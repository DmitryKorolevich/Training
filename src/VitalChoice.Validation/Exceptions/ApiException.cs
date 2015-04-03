using System;
using System.Collections.Generic;
using System.Net;

namespace VitalChoice.Validation.Exceptions
{
    public class ApiException: Exception
    {
        protected readonly HttpStatusCode HttpStatus = HttpStatusCode.InternalServerError;

        private readonly string _errorMessageKey = "Api.Exception.UserFriendlyError";
        private IEnumerable<object> _args; 

        public HttpStatusCode Status
        {
            get { return HttpStatus; }
        }

        public static string GetDefaultErrorMessage
        {
            get { return "Oops something went wrong!"; }
        }

        public ApiException() {}

        public ApiException(string messageKey,IEnumerable<object> args=null)
        {
            _errorMessageKey = messageKey;
            _args = args;
        }

        public ApiException(string messageKey, HttpStatusCode status, IEnumerable<object> args = null)
        {
            _errorMessageKey = messageKey;
            HttpStatus = status;
            _args = args;
        }

        public override string Message
        {
            get
            {
                throw new NotImplementedException();
            }
        }
    }
}
