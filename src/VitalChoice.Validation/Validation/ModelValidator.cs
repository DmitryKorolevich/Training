using System.Collections.Generic;
using FluentValidation.Results;
using VitalChoice.Validation.Models.Interfaces;
using VitalChoice.Validation.Validation.Interfaces;

namespace VitalChoice.Validation.Validation
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
            IsValid = validationResult.IsValid;
            if (!IsValid) {
                foreach (var validationError in validationResult.Errors) {
                    ValidationErrors.Add(validationError.PropertyName, validationError.ErrorMessage);
                }
            }
        }

        void IModelValidator.Validate(IModel value)
        {
            Validate((T)value);
        }

        public abstract void Validate(T value);

        public virtual bool IsValid { get; private set; }
        public virtual IEnumerable<KeyValuePair<string, string>> Errors { get { return ValidationErrors; } }
    }
}