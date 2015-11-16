namespace VitalChoice.Infrastructure.Domain.Entities.Localization.Groups
{
    [LocalizationGroup(1)]
    public enum ValidationMessages : byte
    {
        FieldRequired = 1,
        FieldLength = 2,
        FieldMin = 3,
        FieldMax = 4,
        FieldContentUrlInvalidFormat = 5,
        FieldNumber = 6,
        EmailFormat = 7,
        AtLeastOneRole = 8,
        UserStatusRestriction = 9,
        PasswordMustMatch = 10,
        FieldNameInvalidFormat = 11,
        FieldMinOrEqual = 12,
        FieldMaxOrEqual = 14,
        Exist = 15,
        EmailMustMatch = 16,
        AtLeastOneItem = 17,
        AtLeastOnePaymentMethod = 18,
        AtLeastOneOrderNote = 19,
        MonthFormat = 20,
        YearFormat = 21
    }
}