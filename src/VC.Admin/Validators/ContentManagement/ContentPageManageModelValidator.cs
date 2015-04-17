using System;
using VitalChoice.Validation.Logic;
using FluentValidation;
using VitalChoice.Admin.Models;
using VitalChoice.Domain.Entities.Content;
using VitalChoice.Models.ContentManagement;
using VitalChoice.Domain.Entities.Localization.Groups;
using VitalChoice.Validation.Helpers;

namespace VitalChoice.Validators.ContentManagement
{
    public class ContentPageManageModelValidator : ModelValidator<ContentPageManageModel>
    {
        public override void Validate(ContentPageManageModel value)
        {
            ValidationErrors.Clear();
            ParseResults(ValidatorsFactory.GetValidator<ContentPageModelValidator>().Validate(value));
        }

        private class ContentPageModelValidator : AbstractValidator<ContentPageManageModel>
        {
            public ContentPageModelValidator()
            {
                RuleFor(model => model.Name)
                    .NotEmpty()
                    .WithMessage(model => model.Name, ValidationMessages.FieldRequired);

                RuleFor(model => model.Url)
                    .Cascade(CascadeMode.StopOnFirstFailure)
                    .NotEmpty()
                    .WithMessage(model => model.Url, ValidationMessages.FieldRequired)
                    .Matches(ValidationPatterns.ContentUrlPattern)
                    .WithMessage(model => model.Url, ValidationMessages.FieldContentUrlInvalidFormat);

                RuleFor(model => model.Description)
                    .NotEmpty()
                    .WithMessage(model => model.Description, ValidationMessages.FieldRequired);
            }
        }
    }
}