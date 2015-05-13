using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Newtonsoft.Json;
using VitalChoice.Validation.Attributes;
using VitalChoice.Validation.Logic.Interfaces;
using VitalChoice.Validation.Models.Interfaces;

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
            Validator?.Validate(value);
        }

        [JsonIgnore]
        public virtual bool IsValid => Validator == null || Validator.IsValid;

        [JsonIgnore]
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

        [JsonIgnore]
        public IModelValidator Validator { get; }

        public virtual void Validate()
        {
            Validator?.Validate(this);
        }

        [JsonIgnore]
        public TViewMode Mode
        {
            get { return (TViewMode)(this as IModel).ModeData; }
            set { (this as IModel).ModeData = value; }
        }

        [JsonIgnore]
        IMode IModel.ModeData { get; set; }
    }
}