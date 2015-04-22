﻿using System;

namespace VitalChoice.Domain.Entities.Localization.Groups
{
    [LocalizationGroup(3)]
    public enum GeneralFieldNames : byte
    {
        Name=1,
        Template=2,
        Url = 3,
        Title = 4,
        Description = 5,
        Date = 6,
    }
}