using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNet.Mvc.ModelBinding;
using VitalChoice.Ecommerce.Domain.Exceptions;

namespace VitalChoice.Validation.Models
{
    public static class ResultHelper
    {
        public static Result<T> CreateErrorResult<T>(Exception error, T data = default(T))
        {
            var result = new Result<T>(false, data);
            result.AddMessage(error.GetType().Name, error.Message);
            return result;
        }

        public static Result<T> CreateErrorResult<T>(string errorMessage, T data = default(T))
        {
            var result = new Result<T>(false, data);
            result.AddMessage(string.Empty, errorMessage);
            return result;
        }

        public static Result<T> CreateErrorResult<T>(string errorMessage, string command, T data = default(T))
        {
            var result = new Result<T>(false, data, command);
            result.AddMessage(string.Empty, errorMessage);
            return result;
        }

        public static Result<T> CreateErrorResult<T>(MessageInfo messageInfo, string command = null, T data = default(T))
        {
            var result = new Result<T>(false, data, command);
            result.AddMessage(messageInfo.Field, messageInfo.Message);
            return result;
        }

        public static Result<T> CreateErrorResult<T>(ICollection<MessageInfo> messages, T data = default(T))
        {
            var result = new Result<T>(false, data);
            foreach (var messageInfo in messages.Where(m => m.MessageType == MessageType.FormField))
            {
                result.AddMessage(messageInfo.Field, messageInfo.Message);
            }
            foreach (var messageInfo in messages.Where(m => m.MessageType == MessageType.FieldAsCode))
            {
                if (messageInfo.Field == "ConcurrencyFailure")
                {
                    result.AddMessage(string.Empty, "The data has been changed, please Reload page to see changes");
                }
                else
                {
                    result.AddMessage(string.Empty, messageInfo.Message);
                }
            }
            return result;
        }

        public static Result<T> CreateErrorResult<T>(ModelStateDictionary modelState, T data = default(T))
        {
            var result = new Result<T>(false, data);
            foreach (var state in modelState)
            {
                foreach (var error in state.Value.Errors)
                {
                    result.AddMessage(state.Key, error.ErrorMessage);
                }
            }
            return result;
        }

        public static Result<T> CreateSuccessResult<T>(ModelStateDictionary modelState, T data = default(T))
        {
            var result = new Result<T>(true, data);
            foreach (var state in modelState)
            {
                foreach (var error in state.Value.Errors)
                {
                    result.AddMessage(state.Key, error.ErrorMessage);
                }
            }
            return result;
        }

        public static Result<T> CreateSuccessResult<T>(T data = default(T))
        {
            return new Result<T>(true, data);
        }
    }
}