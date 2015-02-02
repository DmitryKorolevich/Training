using System.Net;

namespace VitalChoice.Validation.Exceptions
{
    public class AccessDeniedException: ApiException
    {
        public AccessDeniedException() : base("Api.Exception.Base.Forbidden", HttpStatusCode.Forbidden) { }

        public AccessDeniedException(string messageKey)
            : base(messageKey, HttpStatusCode.Forbidden) {}
        
    }
}