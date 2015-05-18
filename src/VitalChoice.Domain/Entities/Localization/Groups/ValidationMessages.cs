using System;

namespace VitalChoice.Domain.Entities.Localization.Groups
{
    [LocalizationGroup(1)]
    public enum ValidationMessages : byte
    {
        FieldRequired=1,
        FieldLength=2,
        FieldMin=3,
        FieldMax = 4,
        FieldContentUrlInvalidFormat = 5,
        FieldNumber = 6,
		EmailFormat = 7,
		AtLeastOneRole = 8,
		UserStatusRestriction = 9,
		PasswordMustMatch = 10,
        FieldNameInvalidFormat =11,
        FieldMinOrEqual = 12,
        FieldMaxOrEqual = 14,
    }
}