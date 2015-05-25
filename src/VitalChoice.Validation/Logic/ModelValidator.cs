using System.Collections.Generic;
using FluentValidation.Results;
using VitalChoice.Validation.Logic.Interfaces;
using VitalChoice.Validation.Models.Interfaces;

namespace VitalChoice.Validation.Logic
{
    public abstract class ModelValidator<T> : IModelValidator
        where T: IModel
    {
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
                if (!string.IsNullOrEmpty(propertyPrefixPath))
                {
                    propertyPrefixPath += ".";
                }
                else
                {
                    propertyPrefixPath = string.Empty;
                }
                foreach (var validationError in validationResult.Errors)
                {
                    ValidationErrors.Add(string.Format("{0}{1}.{3}{2}", collectionName, index, validationError.PropertyName, propertyPrefixPath), validationError.ErrorMessage);
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