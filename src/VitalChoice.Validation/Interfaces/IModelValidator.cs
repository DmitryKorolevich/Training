using System.Collections.Generic;

namespace VitalChoice.Validation.Interfaces
{
    public interface IModelValidator
    {
        void Validate(IModel value);
        bool IsValid { get; }
        IEnumerable<KeyValuePair<string, string>> Errors { get; }
    }
}