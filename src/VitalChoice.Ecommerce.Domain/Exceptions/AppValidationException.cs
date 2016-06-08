using System;
using System.Collections.Generic;
using System.Linq;

namespace VitalChoice.Ecommerce.Domain.Exceptions
{
    public class AppValidationException : Exception
    {
        public string ViewName { get; set; }
        public List<MessageInfo> Messages = new List<MessageInfo>();

        public AppValidationException(IEnumerable<MessageInfo> messages) : base("See messages")
        {
            this.Messages.AddRange(messages);
        }

        public AppValidationException(string field, string message) : base("See messages")
        {
            Messages.Add(new MessageInfo()
            {
                Field = field,
                Message = message,
            });
        }

        public AppValidationException(MessageInfo messages) : base("See messages")
        {
            Messages.Add(messages);
        }

        public AppValidationException(string message) : base("See messages")
        {
            Messages.Add(new MessageInfo()
            {
                Field = String.Empty,
                Message = message,
            });
        }

        public override string ToString()
        {
            return $"{string.Join("\n", Messages.Select(m => m.ToString()))}\n{base.ToString()}";
        }
    }
}