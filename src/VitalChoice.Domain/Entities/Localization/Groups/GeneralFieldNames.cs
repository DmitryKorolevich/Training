﻿using System;

namespace VitalChoice.Domain.Entities.Localization.Groups
{
    [LocalizationGroup(3)]
    public enum GeneralFieldNames : byte
    {
        Name = 1,
        Template = 2,
        Url = 3,
        Title = 4,
        Description = 5,
        Date = 6,
        CountryName = 7,
        CountryCode = 8,
        StateName = 9,
        StateCode = 10,
        Email = 11,
        Password = 12,
        FirstName = 13,
        LastName = 14,
        ConfirmPassword = 15,
        Roles = 16,
        UserStatus = 17,
        AgentId = 18,
        OldPassword = 19,
        NewPassword = 20,
        ConfirmNewPassword = 21,
        Min = 45,
        Max = 47,
        Percent = 48,
        Code = 49,
        Quantity = 50,
        Amount = 51,
        Message = 52,
        SKU = 54,
        RetailPrice = 55,
        WholesalePrice = 56,
        Label = 57,
        OrderNote = 58,
        NameOnCard = 59,
        CardType = 60,
        CardNumber = 61,
        ExpirationDateMonth = 62,
        ExpirationDateYear = 63
    }
}