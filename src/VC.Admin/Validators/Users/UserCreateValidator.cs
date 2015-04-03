using System;
using VitalChoice.Validation.Logic;
using FluentValidation;
using VitalChoice.Admin.Models;

namespace VitalChoice.Validators.Users
{
    public class UserCreateValidator: ModelValidator<UserCreateModel>
    {
        public override void Validate(UserCreateModel value)
        {
            ValidationErrors.Clear();
            switch (value.Mode.Mode) {
                case UserCreateMode.CreateAdmin:
                    ParseResults(ValidatorsFactory.GetValidator<UserCreateAdminValidator>().Validate(value));
                    break;
                case UserCreateMode.CreateStandardUser:
                    ParseResults(ValidatorsFactory.GetValidator<UserCreateStandardValidator>().Validate(value, ruleSet:"Main"));
                    if (value.Mode.ShowAccountType)
                    {
                        ParseResults(ValidatorsFactory.GetValidator<UserCreateStandardValidator>()
                                         .Validate(value, ruleSet: "WithAccountType"));
                    }
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}