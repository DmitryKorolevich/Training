using System.Collections.Generic;
using VitalChoice.Validation.Models.Interfaces;

namespace VitalChoice.Validation.Interfaces
{
    public interface IModelValidator
    {
        void Validate(IModel value);
        bool IsValid { get; }
        IEnumerable<KeyValuePair<string, string>> Errors { get; }
    }
}