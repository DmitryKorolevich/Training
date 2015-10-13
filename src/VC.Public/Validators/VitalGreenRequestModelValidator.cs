using FluentValidation;
using VitalChoice.Domain.Entities.Localization.Groups;
using VitalChoice.Validation.Logic;
using VitalChoice.Validation.Helpers;
using VitalChoice.Core.Infrastructure.Helpers;
using VitalChoice.Domain.Entities.eCommerce.Discounts;
using VitalChoice.Domain.Constants;
using VC.Public.Models.Auth;
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