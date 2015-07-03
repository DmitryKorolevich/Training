using System;
using System.Collections.Generic;

namespace VitalChoice.Domain.Exceptions
{
    public class AppValidationException : Exception
    {
        public List<MessageInfo> Messages = new List<MessageInfo>();

        public AppValidationException(IEnumerable<MessageInfo> messages) : base("See messages")
        {
            this.Messages.AddRange(messages);
        }

        public AppValidationException(string field,string message) : base("See messages")
        {
            Messages.Add(new MessageInfo()
            {
                Field=field,
                Message=message,
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
    }
}