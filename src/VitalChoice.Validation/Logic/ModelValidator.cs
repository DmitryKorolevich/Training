﻿using System.Linq;
using System.Collections.Generic;
using FluentValidation.Results;
using VitalChoice.Ecommerce.Domain.Exceptions;
using VitalChoice.Validation.Interfaces;

namespace VitalChoice.Validation.Logic
{
    public abstract class ModelValidator<T> : IModelValidator
        where T: IModel
    {
        protected readonly List<KeyValuePair<string, string>> ValidationErrors;
        protected ModelValidator()
        {
            ValidationErrors = new List<KeyValuePair<string, string>>();
        }

        protected virtual void ParseResults(ValidationResult validationResult, string formName = null)
        {
            IsValid = IsValid && validationResult.IsValid;
            if (!IsValid) {
                foreach (var validationError in validationResult.Errors)
                {
                    ValidationErrors.Add(
                        new KeyValuePair<string, string>(
                            ErrorFieldFormatter.Form(formName, validationError.PropertyName),
                            validationError.ErrorMessage));
                }
            }
        }

        protected virtual void ParseResults(ValidationResult validationResult, string collectionName,int index, string formName = null)
        {
            IsValid = IsValid && validationResult.IsValid;
            if (!IsValid)
            {
                foreach (var validationError in validationResult.Errors)
                {
                    ValidationErrors.Add(
                        new KeyValuePair<string, string>(
                            ErrorFieldFormatter.Form(formName, ErrorFieldFormatter.Collection(collectionName, index, validationError.PropertyName)),
                            validationError.ErrorMessage));
                }
            }
        }

        protected virtual void ParseResults(IModelValidator innerValidator)
        {
            IsValid = IsValid && innerValidator.IsValid;
            if (!IsValid && innerValidator.Errors.Any())
            {
                ValidationErrors.AddRange(innerValidator.Errors);
            }
        }

        void IModelValidator.Validate(IModel value)
        {
            Validate((T)value);
        }

        public abstract void Validate(T value);

        public virtual bool IsValid { get; private set; } = true;

        public virtual IEnumerable<KeyValuePair<string, string>> Errors => ValidationErrors;
    }
}