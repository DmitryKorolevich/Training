using System.Net;

namespace VitalChoice.Ecommerce.Domain.Exceptions
{
    public class NotFoundException : ApiException
    {
        public NotFoundException() : base("Not Found", HttpStatusCode.NotFound) { }

        public NotFoundException(string message) : base(message, HttpStatusCode.NotFound) { }
    }
}