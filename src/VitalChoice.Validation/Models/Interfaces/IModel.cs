using VitalChoice.Validation.Validation.Interfaces;

namespace VitalChoice.Validation.Models.Interfaces
{
    public interface IModel
    {
        IModelValidator Validator { get; }
        IMode ModeData { get; set; }
    }
}