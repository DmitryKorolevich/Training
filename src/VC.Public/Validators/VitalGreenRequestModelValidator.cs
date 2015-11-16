using FluentValidation;
using VitalChoice.Validation.Logic;
using VC.Public.Models;

namespace VC.Public.Validators
{
    public class VitalGreenRequestModelValidator : ModelValidator<VitalGreenRequestModel>
    {
        public override void Validate(VitalGreenRequestModel value)
        {
            ValidationErrors.Clear();
            ParseResults(ValidatorsFactory.GetValidator<VitalGreenRequestInnerModel>().Validate(value));
        }

        private class VitalGreenRequestInnerModel : AbstractValidator<VitalGreenRequestModel>
        {
            public VitalGreenRequestInnerModel()
            {
            }
        }
    }
}