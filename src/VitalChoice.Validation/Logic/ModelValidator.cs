﻿using System.Collections.Generic;
using FluentValidation.Results;
using VitalChoice.Domain.Exceptions;
using VitalChoice.Validation.Helpers;
using VitalChoice.Validation.Interfaces;
using VitalChoice.Validation.Logic.Interfaces;
using VitalChoice.Validation.Models.Interfaces;

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

        

        void IModelValidator.Validate(IModel value)
        {
            Validate((T)value);
        }

        public abstract void Validate(T value);

        public virtual bool IsValid { get; private set; } = true;

        public virtual IEnumerable<KeyValuePair<string, string>> Errors => ValidationErrors;
    }
}