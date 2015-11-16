using System.Collections.Generic;

namespace VitalChoice.Infrastructure.Domain.Constants
{
    public static class ErrorMessagesLibrary
    {
	    public static class Keys
	    {
		    public const string UpdateUserGeneral = "UpdateUserGeneral";
			public const string CantFindUserByActivationToken = "CantFindUserByActivationToken";
		    public const string UserAlreadyConfirmed = "UserAlreadyConfirmed";
		    public const string ActivationTokenExpired = "ActivationTokenExpired";
		    public const string UserLockedOut = "UserLockedOut";
		    public const string IncorrectUserPassword = "IncorrectUserPassword";
		    public const string CantFindUser = "CantFindUser";
		    public const string CantFindLogin = "CantFindLogin";
		    public const string CantSignIn = "CanSignIn";
		    public const string UserIsDisabled = "UserIsDisabled";
            public const string EmailIsTakenAlready = "EmailTakenAlready";
            public const string AgentIdIsTakenAlready = "AgentIdIsTakenAlready";
			public const string CurrentUserRemoval = "CurrentUserRemoval";
			public const string CurrentUserStatusUpdate = "CurrentUserStatusUpdate";
			public const string GenerateSecurityStampError = "GenerateSecurityStampError";
			public const string UserIsNotConfirmed = "UserIsNotConfirmed";
		    public const string CantFindRecord = "CantFindRecord";
		    public const string TitleTakenAlready = "TitleTakenAlready";
		    public const string LabelTakenAlready = "LabelTakenAlready";
		    public const string HasAssignments = "HasAssignments";
		    public const string AtLeastOneDefaultShipping = "AtLeastOneDefaultShipping";
		    public const string AttemptToUpdateUsingWrongService = "AttemptToUpdateUsingWrongService";
		    public const string AttemptToAssignWrongRole = "AttemptToAssignWrongRole";
		    public const string IncorrectCustomerRole = "IncorrectCustomerRole";
		    public const string EmailIsTakenPleaseLogon = "EmailIsTakenPleaseLogon";
		    public const string SuspendedCustomer = "SuspendedCustomer";
		    public const string PasswordRequiresSpecialCharacter = "PasswordRequiresSpecialCharacter";
		    public const string InvalidToken = "InvalidToken";
            public const string InvalidIdAffiliate = "InvalidIdAffiliate";
            public const string ObjectNotFound = "ObjectNotFound";
            public const string AffiliateMinPayCommisionsAmountNotMatch = "AffiliateMinPayCommisionsAmountNotMatch";
        }

	    public static Dictionary<string, string> Data => new Dictionary<string, string>()
	    {
		    {Keys.UpdateUserGeneral, "Error occurred while updating user"},
		    {Keys.CantFindUserByActivationToken, "Invalid authentication token used to identify user in the database"},
		    {Keys.UserAlreadyConfirmed, "User already activated or deleted"},
		    {Keys.ActivationTokenExpired, "Activation token has expired"},
		    {Keys.UserLockedOut, "User is locked out"},
		    {Keys.IncorrectUserPassword, "Incorrect email or password"},
		    {Keys.CantFindUser, "User not found"},
		    {Keys.CantFindLogin, "Login information not found"},
		    {Keys.CantSignIn, "User cannot be signed in"},
		    {Keys.UserIsDisabled, "User is disabled"},
            {Keys.EmailIsTakenAlready, "Email '{0}' is already in use"},
            {Keys.AgentIdIsTakenAlready, "Agent ID is already in use"},
			{Keys.CurrentUserRemoval, "Currently logged in user can't be deleted"},
			{Keys.CurrentUserStatusUpdate, "Status of currently logged in user can't be updated"},
			{Keys.GenerateSecurityStampError, "Cannot generate security token"},
			{Keys.UserIsNotConfirmed, "User not confirmed"},
			{Keys.CantFindRecord, "Record not found"},
			{Keys.TitleTakenAlready, "Title is already in use"},
			{Keys.LabelTakenAlready, "Label is already in use"},
			{Keys.HasAssignments, "Record cannot be removed because there are references to it"},
			{Keys.AtLeastOneDefaultShipping, "At least one shipping address should be default"},
			{Keys.AttemptToUpdateUsingWrongService, "Attempt to update user using forbidden operation"},
			{Keys.AttemptToAssignWrongRole, "Attempt to assign forbidden role"},
			{Keys.IncorrectCustomerRole, "Incorrect customer type"},
			{Keys.EmailIsTakenPleaseLogon, "Email '{0}' is already registered"},
			{Keys.SuspendedCustomer, "Operation can not be completed since associated customer is suspended"},
			{Keys.PasswordRequiresSpecialCharacter, "Passwords must contain at least 1 special character."},
			{Keys.InvalidToken, "Authentication token that has been sent to your email either is expired or regenerated already"},
            {Keys.InvalidIdAffiliate, "Affiliate with the given Id cannot be found"},
            {Keys.ObjectNotFound, "Object cannot be found"},
            {Keys.AffiliateMinPayCommisionsAmountNotMatch, "Affiliate balance is less than ${0}"},
        };
    }
}