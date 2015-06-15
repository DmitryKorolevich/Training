using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace VitalChoice.Domain.Exceptions
{
    public class ApiValidationException : ApiException
    {
        public ApiValidationException()
        {
        }

        public ApiValidationException(string message, HttpStatusCode status) : base(message, status)
        {
        }

        public ApiValidationException(string message, params object[] args) : base(message, args)
        {
        }

        public ApiValidationException(string message, HttpStatusCode status, params object[] args) : base(message, status, args)
        {
        }

        public List<MessageInfo> Messages { get; set; } = new List<MessageInfo>();

        public ApiValidationException(IEnumerable<MessageInfo> messages) : base("See messages")
        {
            Messages.AddRange(messages);
        }

        public ApiValidationException(string field, string message) : base("See messages")
        {
            Messages.Add(new MessageInfo
            {
                Field = field,
                Message = message,
            });
        }
    }
}
