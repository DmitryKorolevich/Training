using System;
using System.Collections.Generic;
using System.Linq;

namespace VitalChoice.Validation.Models
{
    public static class Result
    {
        public static Result<T> CreateErrorResult<T>(Exception error, T data = default(T))
        {
            var result = new Result<T>(false);
            result.AddMessage(error.GetType().Name, error.Message);
            return result;
        }        

        public static Result<T> CreateErrorResult<T>(string errorMessage, T data = default(T))
        {
            var result = new Result<T>(false);
            result.AddMessage("", errorMessage );
            return result;
        }

        public static Result<T> CreateSuccessResult<T>(T data = default(T))
        {
            return new Result<T>(true, data);
        }
    }

    [DataContract]
    public struct Result<T>
    {
        private readonly List<MessageInfo> _messages;
        private readonly T _data;
        private readonly bool _success;

        public Result(bool status, T data = default(T))
        {
            _messages = new List<MessageInfo>();
            _data = data;
            _success = status;
        }

        [DataMember]
        public T Data
        {
            get { return _data; }
        }

        [DataMember]
        public bool Success
        {
            get { return _success; }
        }

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
        public IEnumerable<MessageInfo> Messages
        {
            get { return _messages.AsEnumerable(); }
        }

        ///Default to success upon implicit conversion
        public static implicit operator Result<T>(T value)
        {
            return new Result<T>(true, value);
        }
    }
}