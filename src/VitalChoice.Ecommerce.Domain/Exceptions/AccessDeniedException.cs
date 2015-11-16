using System.Net;

namespace VitalChoice.Ecommerce.Domain.Exceptions
{
    public class AccessDeniedException: ApiException
    {
        public AccessDeniedException() : base("Api.Exception.Base.Forbidden", HttpStatusCode.Forbidden) { }

        public AccessDeniedException(string message)
            : base(message, HttpStatusCode.Forbidden) {}
        
    }
}