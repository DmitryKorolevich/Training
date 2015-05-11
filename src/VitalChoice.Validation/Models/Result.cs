using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using VitalChoice.Domain.Exceptions;

namespace VitalChoice.Validation.Models
{
    public struct Result<T>
    {
        private readonly List<MessageInfo> _messages;

        public Result(bool status, T data = default(T),string command=null)
        {
            _messages = new List<MessageInfo>();
            Data = data;
            Success = status;
            Command = command;
        }

        public T Data { get; }

        public bool Success { get; }

        public string Command { get; }

        public void AddMessage(string field, string message)
        {
            _messages.Add(new MessageInfo
            {
                Field = field,
                Message = message
            });
        }

        public void AddMessages(IEnumerable<MessageInfo> messages)
        {
            _messages.AddRange(messages);
        }

        public void AddMessages(params MessageInfo[] messages)
        {
            _messages.AddRange(messages);
        }

        public void ClearMessages()
        {
            _messages.Clear();
        }

        [DataMember]
        public IEnumerable<MessageInfo> Messages => _messages.AsEnumerable();

        ///Default to success upon implicit conversion
        public static implicit operator Result<T>(T value)
        {
            return new Result<T>(true, value);
        }
    }
}