using System;
using VitalChoice.Validation.Logic;
using FluentValidation;
using VitalChoice.Admin.Models;
using VitalChoice.Domain.Entities.Content;
using VitalChoice.Models.ContentManagement;
using VitalChoice.Domain.Entities.Localization.Groups;
using VitalChoice.Validation.Helpers;
using VitalChoice.Models.Setting;

namespace VitalChoice.Validators.Setting
{
    public class CountryManageModelValidator : ModelValidator<CountryManageModel>
    {
        public override void Validate(CountryManageModel value)
        {
            ValidationErrors.Clear();
            ParseResults(ValidatorsFactory.GetValidator<CountryModelValidator>().Validate(value));
        }

        private class CountryModelValidator : AbstractValidator<CountryManageModel>
        {
            public CountryModelValidator()
            {
                RuleFor(model => model.CountryCode)
                    .NotEmpty()
                    .WithMessage(model => model.CountryCode, ValidationMessages.FieldRequired);

                RuleFor(model => model.CountryName)
                    .NotEmpty()
                    .WithMessage(model => model.CountryName, ValidationMessages.FieldRequired);
            }
        }
    }
}