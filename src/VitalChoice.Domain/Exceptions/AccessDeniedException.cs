using System.Net;

namespace VitalChoice.Domain.Exceptions
{
    public class AccessDeniedException: ApiException
    {
        public AccessDeniedException() : base("Api.Exception.Base.Forbidden", HttpStatusCode.Forbidden) { }

        public AccessDeniedException(string message)
            : base(message, HttpStatusCode.Forbidden) {}
        
    }
}