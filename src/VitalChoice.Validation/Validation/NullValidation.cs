using VitalChoice.Validation.Models.Interfaces;

namespace VitalChoice.Validation.Validation
{
    public class NullValidation : ModelValidator<IModel>
    {
        public override void Validate(IModel value)
        {
            ValidationErrors.Clear();
        }
    }
}
