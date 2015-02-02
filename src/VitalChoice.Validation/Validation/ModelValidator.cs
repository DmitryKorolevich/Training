using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using FluentValidation.Results;
using VitalChoice.Validation.Models.Interfaces;
using VitalChoice.Validation.Validation.Interfaces;

namespace VitalChoice.Validation.Validation
{
    public abstract class ModelValidator<T> : IModelValidator
        where T: IModel
    {
        private bool _isValid = true;

        protected readonly Dictionary<string, string> ValidationErrors;
        protected ModelValidator()
        {
            ValidationErrors = new Dictionary<string, string>();
        }

        protected virtual void ParseResults(ValidationResult validationResult)
        {
            IsValid = IsValid && validationResult.IsValid;
            if (!IsValid) {
                foreach (var validationError in validationResult.Errors) {
                    ValidationErrors.Add(validationError.PropertyName , validationError.ErrorMessage);
                }
            }
        }

        protected virtual void ParseResults(ValidationResult validationResult,string collectionName,int index,string propertyPrefixPath=null)
        {
            IsValid = IsValid && validationResult.IsValid;
            if (!IsValid)
            {
                if (!String.IsNullOrEmpty(propertyPrefixPath))
                {
                    propertyPrefixPath += ".";
                }
                else
                {
                    propertyPrefixPath = String.Empty;
                }
                foreach (var validationError in validationResult.Errors)
                {
                    ValidationErrors.Add(String.Format("{0}.{1}.{3}{2}", collectionName, index, validationError.PropertyName, propertyPrefixPath), validationError.ErrorMessage);
                }
            }
        }

        void IModelValidator.Validate(IModel value)
        {
            Validate((T)value);
        }

        public abstract void Validate(T value);

        public virtual bool IsValid
        {
            get { return _isValid; }
            private set { _isValid = value; }
        }
        public virtual IEnumerable<KeyValuePair<string, string>> Errors { get { return ValidationErrors; } }
    }
}