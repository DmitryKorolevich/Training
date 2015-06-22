using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using FluentValidation;
using VitalChoice.Business.Services;
using VitalChoice.Domain.Entities.Localization.Groups;
using VitalChoice.Validation.Models;
using VitalChoice.Domain.Constants;

namespace VitalChoice.Core.Infrastructure.Helpers
{
    public static class LocalizedMessages
    {
        public static string GetMessage<TEnum>(TEnum key, IEnumerable<object> args) where TEnum : IComparable
        {
            return LocalizationService.GetString(key, args.ToArray());
        }

        private static string GetFieldName<TEnum>(TEnum key) where TEnum : IComparable
        {
            return LocalizationService.GetString(key);
        }

        public static string Do()
        {
            var data = LocalizationService.GetString(ValidationMessages.FieldLength);
            return data;
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
        public static IRuleBuilderOptions<TModel, TResult> WithMessage<TModel, TEnum, TResult>(
            this IRuleBuilderOptions<TModel, TResult> optionsChain,
            Expression<Func<TModel, TResult>> expression, TEnum messageKey) where TEnum : IComparable
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
            List<object> parameters = new List<object>();
            if (attribute != null)
            {
                parameters.Add(GetFieldName((IComparable)attribute.EnumValue));
            }
            else
            {
                parameters.Add(BaseAppConstants.DEFAULT_FORM_FIELD_NAME);
            }

            return optionsChain.WithMessage(GetMessage(messageKey, parameters));
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
        public static IRuleBuilderOptions<TModel, TResult> WithMessage<TModel, TEnum, TResult>(
            this IRuleBuilderOptions<TModel, TResult> optionsChain,
            Expression<Func<TModel, TResult>> expression, TEnum messageKey, params object[] args) where TEnum : IComparable
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
            List<object> parameters = new List<object>();
            if (attribute != null)
            {
                parameters.Add(GetFieldName((IComparable)attribute.EnumValue));
            }
            else
            {
                parameters.Add(BaseAppConstants.DEFAULT_FORM_FIELD_NAME);
            }
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
        public static IRuleBuilderOptions<TModel, TResult> WithMessage<TModel, TEnum, TEnum2, TResult>(
            this IRuleBuilderOptions<TModel, TResult> optionsChain,
            TEnum messageKey, TEnum2 fieldKey, params object[] args) where TEnum : IComparable
                                                                     where TEnum2: IComparable
        {
            List<object> parameters = new List<object> { GetFieldName(fieldKey) };
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
        /// <param name="fieldKey">Custom user field key(label).</param>
        /// <param name="messageKey">Message key, local name in Validation.Global.Messages. No need to write full name.</param>
        /// <param name="args">Additional parameters</param>
        /// <returns>Validation chain</returns>
        public static IRuleBuilderOptions<TModel, TResult> WithMessageWithCustomFieldLabel<TModel, TEnum, TEnum2, TResult>(
            this IRuleBuilderOptions<TModel, TResult> optionsChain,
            TEnum messageKey, TEnum2 fieldKey, params object[] args) where TEnum : IComparable
                                                                     where TEnum2 : IComparable
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
        public static IRuleBuilderOptions<TModel, TResult> WithMessageWithParams<TModel, TEnum, TEnum2, TResult>(
            this IRuleBuilderOptions<TModel, TResult> optionsChain,
            TEnum2 messageKey, params object[] args) where TEnum : IComparable
                                                     where TEnum2 : IComparable
        {
            if (args == null)
            {
                return optionsChain.WithMessage(GetMessage(messageKey, new object[0]));
            }
            return optionsChain.WithMessage(GetMessage(messageKey, args));
        }
    }
}