using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using FluentValidation;
using VitalChoice.Validation.Models;

namespace VitalChoice.Validation.Validation
{
    public static class LocalizedMessages
    {

        private const string ValidationGlobalFieldsNamespace = "Common.Validation.Fields.";
        private const string ValidationApiFieldsNamespace = "Api.Validation.Fields.";
        private const string ValidationGlobalMessagesNamespace = "Common.Validation.Messages.";
        private const string ValidationApiMessagesNamespace = "Api.Validation.Messages.";

        public static string GetMessage(string key, ValidationScope scope, IEnumerable<object> args)
        {
            switch (scope)
            {
                case ValidationScope.Common:
                    //return CommonManager.LocalizationData.GetString(ValidationGlobalMessagesNamespace + key, args);
                case ValidationScope.Api:
                    //return CommonManager.LocalizationData.GetString(ValidationApiMessagesNamespace + key, args);
                default:
                    throw new ArgumentOutOfRangeException("scope");
            }
        }

        private static string GetFieldName(string key, ValidationScope scope)
        {
            switch (scope)
            {
                case ValidationScope.Common:
                    //return CommonManager.LocalizationData.GetString(ValidationGlobalFieldsNamespace + key);
                case ValidationScope.Api:
                    //return CommonManager.LocalizationData.GetString(ValidationApiFieldsNamespace + key);
                default:
                    throw new ArgumentOutOfRangeException("scope");
            }
        }

        /// <summary>
        /// Extends Stadard FluentValidation .WithMessage method. Adds localized validation message in current validation chain.
        /// </summary>
        /// <typeparam name="TModel">Model type</typeparam>
        /// <typeparam name="TResult">Selected Property Type</typeparam>
        /// <param name="optionsChain">Validation Options Chain</param>
        /// <param name="expression">Select Property Expression</param>
        /// <param name="messageKey">Message key, local name in Validation.Global.Messages. No need to write full name.</param>
        /// <param name="scope">Validation scope (API or Common validators)</param>
        /// <returns>Validation chain</returns>
        public static IRuleBuilderOptions<TModel, TResult> WithMessage<TModel, TResult>(
            this IRuleBuilderOptions<TModel, TResult> optionsChain,
            Expression<Func<TModel, TResult>> expression, string messageKey, ValidationScope scope)
        {
            if (expression.Body.NodeType != ExpressionType.MemberAccess)
            {
                throw new InvalidOperationException("Only member select expressions accepted.");
            }
            var memberExpression = (MemberExpression) expression.Body;
            if (!(memberExpression.Member is PropertyInfo))
            {
                throw new InvalidOperationException("Only property members accepted.");
            }
            var attribute =
                memberExpression.Member.GetCustomAttributes(typeof (LocalizedAttribute), false).FirstOrDefault() as
                LocalizedAttribute;
            if (attribute == null)
            {
                throw new ArgumentException(string.Format("LocalizedAttribute not set on property {0}.", memberExpression.Member.Name), memberExpression.Member.Name);
            }
            return optionsChain.WithMessage(GetMessage(messageKey, scope, new object[] { GetFieldName(attribute.PropertyKey, ValidationScope.Api)}));
        }

        /// <summary>
        /// Extends Stadard FluentValidation .WithMessage method. Adds localized validation message in current validation chain.
        /// </summary>
        /// <typeparam name="TModel">Model type</typeparam>
        /// <typeparam name="TResult">Selected Property Type</typeparam>
        /// <param name="optionsChain">Validation Options Chain</param>
        /// <param name="expression">Select Property Expression</param>
        /// <param name="messageKey">Message key, local name in Validation.Global.Messages. No need to write full name.</param>
        /// <param name="scope">Validation scope (API or Common validators)</param>
        /// <param name="args">Additional parameters</param>
        /// <returns>Validation chain</returns>
        public static IRuleBuilderOptions<TModel, TResult> WithMessage<TModel, TResult>(
            this IRuleBuilderOptions<TModel, TResult> optionsChain,
            Expression<Func<TModel, TResult>> expression, string messageKey, ValidationScope scope, params object[] args)
        {
            if (expression.Body.NodeType != ExpressionType.MemberAccess)
            {
                throw new InvalidOperationException("Only member select expressions accepted.");
            }
            var memberExpression = (MemberExpression)expression.Body;
            if (!(memberExpression.Member is PropertyInfo))
            {
                throw new InvalidOperationException("Only property members accepted.");
            }
            var attribute =
                memberExpression.Member.GetCustomAttributes(typeof(LocalizedAttribute), false).FirstOrDefault() as
                LocalizedAttribute;
            if (attribute == null)
            {
                throw new ArgumentException(string.Format("LocalizedAttribute not set on property {0}.", memberExpression.Member.Name), memberExpression.Member.Name);
            }
            List<object> parameters = new List<object> { GetFieldName(attribute.PropertyKey, ValidationScope.Api) };
            if (args != null) {
                parameters.AddRange(args);
            }
            return optionsChain.WithMessage(GetMessage(messageKey, scope, parameters));
        }

        /// <summary>
        /// Extends Stadard FluentValidation .WithMessage method. Adds localized validation message in current validation chain.
        /// </summary>
        /// <typeparam name="TModel">Model type</typeparam>
        /// <typeparam name="TResult">Selected Property Type</typeparam>
        /// <param name="optionsChain">Validation Options Chain</param>
        /// <param name="fieldKey">Field key, local name in Validation.Global.Fields. No need to write full name.</param>
        /// <param name="messageKey">Message key, local name in Validation.Global.Messages. No need to write full name.</param>
        /// <param name="scope">Validation scope (API or Common validators)</param>
        /// <param name="args">Additional parameters</param>
        /// <returns>Validation chain</returns>
        public static IRuleBuilderOptions<TModel, TResult> WithMessage<TModel, TResult>(
            this IRuleBuilderOptions<TModel, TResult> optionsChain,
            string messageKey, string fieldKey, ValidationScope scope, params object[] args)
        {
            List<object> parameters = new List<object> { GetFieldName(fieldKey, scope) };
            if (args != null) {
                parameters.AddRange(args);
            }
            return optionsChain.WithMessage(GetMessage(messageKey, scope, parameters));
        }

        /// <summary>
        /// Extends Stadard FluentValidation .WithMessage method. Adds localized validation message in current validation chain.
        /// </summary>
        /// <typeparam name="TModel">Model type</typeparam>
        /// <typeparam name="TResult">Selected Property Type</typeparam>
        /// <param name="optionsChain">Validation Options Chain</param>
        /// <param name="messageKey">Message key, local name in Validation.Global.Messages. No need to write full name.</param>
        /// <param name="scope">Validation scope (API or Common validators)</param>
        /// <param name="args">Additional parameters</param>
        /// <returns>Validation chain</returns>
        public static IRuleBuilderOptions<TModel, TResult> WithMessage<TModel, TResult>(
            this IRuleBuilderOptions<TModel, TResult> optionsChain,
            string messageKey, ValidationScope scope, params object[] args)
        {
            if (args == null) {
                return optionsChain.WithMessage(GetMessage(messageKey, scope, new object[0]));
            }
            return optionsChain.WithMessage(GetMessage(messageKey, scope, args));
        }
    }
}