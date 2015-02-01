using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using VitalChoice.Validation.Attributes;
using VitalChoice.Validation.Models.Interfaces;
using VitalChoice.Validation.Validation;
using VitalChoice.Validation.Validation.Interfaces;

namespace VitalChoice.Validation.Models
{
    public abstract class Model<T, TViewMode> : IModel, IModelValidator
        where T: new()
        where TViewMode: IMode
    {
        protected Model()
        {
            var validatorAttribute =
                GetType().GetTypeInfo().GetCustomAttributes(typeof(ApiValidatorAttribute), true).SingleOrDefault() as
                ApiValidatorAttribute;
            if (validatorAttribute != null)
                Validator = (IModelValidator)Activator.CreateInstance(validatorAttribute.ValidatorType);
        }

        public virtual T Convert()
        {
            return new T();
        }

        public virtual T Update(T dataToUpdate)
        {
            return Convert();
        }

        public virtual void Validate(IModel value)
        {
            if (Validator != null) {
                Validator.Validate(value);
            }
        }

        public virtual bool IsValid
        {
            get { return Validator == null || Validator.IsValid; }
        }

        public virtual IEnumerable<KeyValuePair<string, string>> Errors
        {
            get
            {
                if (Validator != null) {
                    return Validator.Errors;
                }
                return new List<KeyValuePair<string, string>>();
            }
        }

        public IModelValidator Validator { get; private set; }

        public virtual void Validate()
        {
            if (Validator != null) {
                Validator.Validate(this);
            }
        }

        public TViewMode Mode
        {
            get { return (TViewMode)(this as IModel).ModeData; }
            set { (this as IModel).ModeData = value; }
        }

        IMode IModel.ModeData { get; set; }
    }
}