using VitalChoice.Validation.Interfaces;
using VitalChoice.Validation.Models.Interfaces;

namespace VitalChoice.Validation.Logic
{
    public class NullValidation : ModelValidator<IModel>
    {
        public override void Validate(IModel value)
        {
            ValidationErrors.Clear();
        }
    }
}
