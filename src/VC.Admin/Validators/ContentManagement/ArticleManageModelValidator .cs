﻿using FluentValidation;
using VC.Admin.Models.ContentManagement;
using VitalChoice.Core.Infrastructure.Helpers;
using VitalChoice.Infrastructure.Domain.Constants;
using VitalChoice.Infrastructure.Domain.Entities.Localization.Groups;
using VitalChoice.Validation.Logic;
using VitalChoice.Validation.Helpers;

namespace VC.Admin.Validators.ContentManagement
{
    public class ArticleManageModelValidator : ModelValidator<ArticleManageModel>
    {
        public override void Validate(ArticleManageModel value)
        {
            ValidationErrors.Clear();
            ParseResults(ValidatorsFactory.GetValidator<ArticleModelValidator>().Validate(value));
        }

        private class ArticleModelValidator : AbstractValidator<ArticleManageModel>
        {
            public ArticleModelValidator()
            {
                RuleFor(model => model.Name)
                    .NotEmpty()
                    .WithMessage(model => model.Name, ValidationMessages.FieldRequired)
                    .Length(0, BaseAppConstants.DEFAULT_TEXT_FIELD_EXPANDED_MAX_SIZE)
                    .WithMessage(model => model.Name, ValidationMessages.FieldLength, BaseAppConstants.DEFAULT_TEXT_FIELD_EXPANDED_MAX_SIZE);

                RuleFor(model => model.Url)
                    .Cascade(CascadeMode.StopOnFirstFailure)
                    .NotEmpty()
                    .WithMessage(model => model.Url, ValidationMessages.FieldRequired)
                    .Matches(ValidationPatterns.ContentUrlPattern)
                    .WithMessage(model => model.Url, ValidationMessages.FieldContentUrlInvalidFormat)
                    .Length(0, BaseAppConstants.DEFAULT_TEXT_FIELD_EXPANDED_MAX_SIZE)
                    .WithMessage(model => model.Url, ValidationMessages.FieldLength, BaseAppConstants.DEFAULT_TEXT_FIELD_EXPANDED_MAX_SIZE);

                RuleFor(model => model.Title)
                    .Length(0, BaseAppConstants.DEFAULT_TEXT_FIELD_EXPANDED_MAX_SIZE)
                    .WithMessage(model => model.Title, ValidationMessages.FieldLength, BaseAppConstants.DEFAULT_TEXT_FIELD_EXPANDED_MAX_SIZE);
                RuleFor(model => model.SubTitle)
                    .Length(0, BaseAppConstants.DEFAULT_TEXT_FIELD_EXPANDED_MAX_SIZE)
                    .WithMessage(model => model.SubTitle, ValidationMessages.FieldLength, BaseAppConstants.DEFAULT_TEXT_FIELD_EXPANDED_MAX_SIZE);
                RuleFor(model => model.Author)
                    .Length(0, BaseAppConstants.DEFAULT_TEXT_FIELD_EXPANDED_MAX_SIZE)
                    .WithMessage(model => model.Author, ValidationMessages.FieldLength, BaseAppConstants.DEFAULT_TEXT_FIELD_EXPANDED_MAX_SIZE);

                RuleFor(model => model.Description)
                    .NotEmpty()
                    .WithMessage(model => model.Description, ValidationMessages.FieldRequired);
            }
        }
    }
}