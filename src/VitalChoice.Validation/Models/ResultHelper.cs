using System;
using System.Collections.Generic;
using VitalChoice.Domain.Exceptions;

namespace VitalChoice.Validation.Models
{
    public static class ResultHelper
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

        public static Result<T> CreateErrorResult<T>(string errorMessage,string command, T data = default(T))
        {
            var result = new Result<T>(false, data,command);
            result.AddMessage("", errorMessage);
            return result;
        }

        public static Result<T> CreateErrorResult<T>(MessageInfo messageInfo, string command=null, T data = default(T))
        {
            var result = new Result<T>(false, data, command);
            result.AddMessage(messageInfo.Field, messageInfo.Message);
            return result;
        }

        public static Result<T> CreateErrorResult<T>(IEnumerable<MessageInfo> messages, T data = default(T))
        {
            var result = new Result<T>(false);
            foreach (var messageInfo in messages)
            {
                result.AddMessage(messageInfo.Field,messageInfo.Message);
            }
            return result;
        }

        public static Result<T> CreateSuccessResult<T>(T data = default(T))
        {
            return new Result<T>(true, data);
        }
    }
}