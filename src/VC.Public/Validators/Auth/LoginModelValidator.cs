﻿using FluentValidation;
using VitalChoice.Domain.Entities.Localization.Groups;
using VitalChoice.Validation.Logic;
using VitalChoice.Validation.Helpers;
using VitalChoice.Core.Infrastructure.Helpers;
using VitalChoice.Domain.Entities.eCommerce.Discounts;
using VitalChoice.Domain.Constants;
using VC.Public.Models.Auth;

namespace VC.Public.Validators.Auth
{
    public class LoginModelValidator : ModelValidator<LoginModel>
    {
        public override void Validate(LoginModel value)
        {
            ValidationErrors.Clear();
            ParseResults(ValidatorsFactory.GetValidator<LoginModelInnerValidator>().Validate(value));
        }

        private class LoginModelInnerValidator : AbstractValidator<LoginModel>
        {
            public LoginModelInnerValidator()
            { 
            //    RuleFor(model => model.Email)
            //        .Must(model => model.Length == 10)
            //        .WithMessage(model => model.Email, ValidationMessages.FieldRequired);
            }
        }
    }
}