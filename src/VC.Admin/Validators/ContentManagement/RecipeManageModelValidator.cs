﻿using System;
using VitalChoice.Validation.Logic;
using FluentValidation;
using VitalChoice.Admin.Models;
using VitalChoice.Domain.Entities.Content;
using VitalChoice.Models.ContentManagement;
using VitalChoice.Domain.Entities.Localization.Groups;
using VitalChoice.Validation.Helpers;

namespace VitalChoice.Validators.ContentManagement
{
    public class RecipeManageModelValidator : ModelValidator<RecipeManageModel>
    {
        public override void Validate(RecipeManageModel value)
        {
            ValidationErrors.Clear();
            ParseResults(ValidatorsFactory.GetValidator<RecipeModelValidator>().Validate(value));
        }

        private class RecipeModelValidator : AbstractValidator<RecipeManageModel>
        {
            public RecipeModelValidator()
            {
                RuleFor(model => model.Name)
                    .NotEmpty()
                    .WithMessage(model => model.Name, ValidationMessages.FieldRequired);

                RuleFor(model => model.Url)
                    .NotEmpty()
                    .WithMessage(model => model.Url, ValidationMessages.FieldRequired);
                RuleFor(model => model.Url)
                    .Matches(ValidationPatterns.ContentUrlPattern)
                    .WithMessage(model => model.Url, ValidationMessages.FieldContentUrlInvalidFormat);
            }
        }
    }
}