using System;

namespace VitalChoice.Domain.Entities.Localization.Groups
{
    [LocalizationGroup(1)]
    public enum ValidationMessages
    {
        FieldRequired=1,
        FieldLength=2,
        FieldMin=3,
        FieldMax = 4,
    }
}