using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using VitalChoice.Business.Services;
using VitalChoice.Infrastructure.Domain.Entities.Localization.Groups;

namespace VC.Public.DataAnnotations
{
    public class CustomMaxLengthAttribute : MaxLengthAttribute
    {
        public CustomMaxLengthAttribute(int length)
            : base(length)
        {
            ErrorMessage = LocalizationService.Current.GetString(ValidationMessages.FieldLength);
        }
    }
}
