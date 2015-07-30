using VitalChoice.Validation.Models.Interfaces;

namespace VitalChoice.Validation.Interfaces
{
    public interface IModel
    {
        IModelValidator Validator { get; }
        IMode ModeData { get; set; }
    }
}