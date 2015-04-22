using System;
using System.Collections.Generic;
using System.Net;

namespace VitalChoice.Domain.Exceptions
{
    public class ApiException: Exception
    {
        protected readonly HttpStatusCode HttpStatus = HttpStatusCode.InternalServerError;

        public HttpStatusCode Status => HttpStatus;

        public static string GetDefaultErrorMessage => "Oops something went wrong!";

        public ApiException() {}

        public ApiException(string message, params object[] args) {
            Message = string.Format(message, args);
        }

        public ApiException(string message, HttpStatusCode status, params object[] args)
        {
            Message = string.Format(message, args);
            HttpStatus = status;
        }

        public ApiException(string message, HttpStatusCode status)
        {
            Message = message;
            HttpStatus = status;
        }

        public override string Message { get; }
    }
}
