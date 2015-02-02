using System;
using FluentValidation;

namespace QRProject.Common.ApiBase.Validation
{
    public class FieldMaxSizeValidator : AbstractValidator<string>
    {
        public FieldMaxSizeValidator(int maxLenth,string fieldName,bool customFieldName=false)
        {
            fieldName = String.IsNullOrWhiteSpace(fieldName) ? "Field" : fieldName;
            if(customFieldName)
            {
                RuleFor(model => model)
                    .Length(0, maxLenth).When(p => !String.IsNullOrEmpty(p))
                    .WithMessageWithCustomFieldLabel("FieldMaxSize", fieldName, maxLenth)
                    .WithName(fieldName);
            }
            else
            {
                RuleFor(model => model)
                    .Length(0, maxLenth).When(p => !String.IsNullOrEmpty(p))
                    .WithMessage("FieldMaxSize", fieldName, maxLenth)
                    .WithName(fieldName);
            }
        }
    }
}