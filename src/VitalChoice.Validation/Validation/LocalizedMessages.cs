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
        private const string ValidationGlobalMessagesNamespace = "Common.Validation.Messages.";

        public static string GetMessage(string key, IEnumerable<object> args)
        {
            return CommonManager.LocalizationData.GetString(ValidationGlobalMessagesNamespace + key, args);
        }

        private static string GetFieldName(string key)
        {
            return CommonManager.LocalizationData.GetString(ValidationGlobalFieldsNamespace + key);
        }

        /// <summary>
        /// Extends Stadard FluentValidation .WithMessage method. Adds localized validation message in current validation chain.
        /// </summary>
        /// <typeparam name="TModel">Model type</typeparam>
        /// <typeparam name="TResult">Selected Property Type</typeparam>
        /// <param name="optionsChain">Validation Options Chain</param>
        /// <param name="expression">Select Property Expression</param>
        /// <param name="messageKey">Message key, local name in Validation.Global.Messages. No need to write full name.</param>
        /// <returns>Validation chain</returns>
        public static IRuleBuilderOptions<TModel, TResult> WithMessage<TModel, TResult>(
            this IRuleBuilderOptions<TModel, TResult> optionsChain,
            Expression<Func<TModel, TResult>> expression, string messageKey)
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
            return optionsChain.WithMessage(GetMessage(messageKey, new object[] { GetFieldName(attribute.PropertyKey)}));
        }

        /// <summary>
        /// Extends Stadard FluentValidation .WithMessage method. Adds localized validation message in current validation chain.
        /// </summary>
        /// <typeparam name="TModel">Model type</typeparam>
        /// <typeparam name="TResult">Selected Property Type</typeparam>
        /// <param name="optionsChain">Validation Options Chain</param>
        /// <param name="expression">Select Property Expression</param>
        /// <param name="messageKey">Message key, local name in Validation.Global.Messages. No need to write full name.</param>
        /// <param name="args">Additional parameters</param>
        /// <returns>Validation chain</returns>
        public static IRuleBuilderOptions<TModel, TResult> WithMessage<TModel, TResult>(
            this IRuleBuilderOptions<TModel, TResult> optionsChain,
            Expression<Func<TModel, TResult>> expression, string messageKey, params object[] args)
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
            List<object> parameters = new List<object> { GetFieldName(attribute.PropertyKey) };
            if (args != null)
            {
                parameters.AddRange(args);
            }
            return optionsChain.WithMessage(GetMessage(messageKey, parameters));
        }

        /// <summary>
        /// Extends Stadard FluentValidation .WithMessage method. Adds localized validation message in current validation chain.
        /// </summary>
        /// <typeparam name="TModel">Model type</typeparam>
        /// <typeparam name="TResult">Selected Property Type</typeparam>
        /// <param name="optionsChain">Validation Options Chain</param>
        /// <param name="fieldKey">Field key, local name in Validation.Global.Fields. No need to write full name.</param>
        /// <param name="messageKey">Message key, local name in Validation.Global.Messages. No need to write full name.</param>
        /// <param name="args">Additional parameters</param>
        /// <returns>Validation chain</returns>
        public static IRuleBuilderOptions<TModel, TResult> WithMessage<TModel, TResult>(
            this IRuleBuilderOptions<TModel, TResult> optionsChain,
            string messageKey, string fieldKey, params object[] args)
        {
            List<object> parameters = new List<object> { GetFieldName(fieldKey) };
            if (args != null) {
                parameters.AddRange(args);
            }
            return optionsChain.WithMessage(GetMessage(messageKey, parameters));
        }

        /// <summary>
        /// Extends Stadard FluentValidation .WithMessage method. Adds localized validation message in current validation chain.
        /// </summary>
        /// <typeparam name="TModel">Model type</typeparam>
        /// <typeparam name="TResult">Selected Property Type</typeparam>
        /// <param name="optionsChain">Validation Options Chain</param>
        /// <param name="fieldKey">Custom user field key(label).</param>
        /// <param name="messageKey">Message key, local name in Validation.Global.Messages. No need to write full name.</param>
        /// <param name="args">Additional parameters</param>
        /// <returns>Validation chain</returns>
        public static IRuleBuilderOptions<TModel, TResult> WithMessageWithCustomFieldLabel<TModel, TResult>(
            this IRuleBuilderOptions<TModel, TResult> optionsChain,
            string messageKey, string fieldKey, params object[] args)
        {
            List<object> parameters = new List<object> { fieldKey };
            if (args != null)
            {
                parameters.AddRange(args);
            }
            return optionsChain.WithMessage(GetMessage(messageKey, parameters));
        }

        /// <summary>
        /// Extends Stadard FluentValidation .WithMessage method. Adds localized validation message in current validation chain.
        /// </summary>
        /// <typeparam name="TModel">Model type</typeparam>
        /// <typeparam name="TResult">Selected Property Type</typeparam>
        /// <param name="optionsChain">Validation Options Chain</param>
        /// <param name="messageKey">Message key, local name in Validation.Global.Messages. No need to write full name.</param>
        /// <param name="args">Additional parameters</param>
        /// <returns>Validation chain</returns>
        public static IRuleBuilderOptions<TModel, TResult> WithMessageWithParams<TModel, TResult>(
            this IRuleBuilderOptions<TModel, TResult> optionsChain,
            string messageKey,params object[] args)
        {
            if (args == null) {
                return optionsChain.WithMessage(GetMessage(messageKey, new object[0]));
            }
            return optionsChain.WithMessage(GetMessage(messageKey, args));
        }
    }
}