using System.Net;
using System.Runtime.Serialization;

namespace VitalChoice.Validation.Exceptions
{
    public class AuthenticationFailedException : ApiException
    {
        public AuthenticationFailedException() : base("Api.Exception.App.AuthenticationFailed", HttpStatusCode.Forbidden) { }

        public AuthenticationFailedException(string messageKey)
            : base(messageKey, HttpStatusCode.Forbidden) {}
    }
}