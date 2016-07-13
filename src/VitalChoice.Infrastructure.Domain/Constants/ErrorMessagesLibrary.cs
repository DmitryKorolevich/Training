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
            public const string CantFindUserResetPassword = "CantFindUserResetPassword";
            public const string CantFindLogin = "CantFindLogin";
		    public const string CantSignIn = "CantSignIn";
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
		    public const string AtLeastOneDefaultCreditCard = "AtLeastOneDefaultCreditCard";
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
            public const string WrongCaptcha = "WrongCaptcha";
            public const string CustomerWasModified = "CustomerWasModified";
            public const string SkuNotFound = "SkuNotFound";
            public const string OrderImportRowError = "OrderImportRowError";
            public const string ParseDateError = "ParseDateError";
            public const string ParseIntError = "ParseIntError";
            public const string ZeroSkusForOrderInImport = "ZeroSkusForOrderInImport";
            public const string InvalidFieldValue = "InvalidFieldValue";
            public const string FieldIsRequired = "FieldIsRequired";
            public const string FieldIsInvalidEmail = "FieldIsInvalidEmail";
            public const string FieldMaxLength = "FieldMaxLength";
            public const string SkuNotFoundOrderImport = "SkuNotFoundOrderImport";
            public const string EmptyCart = "EmptyCart";
            public const string MustBeFutureDateError = "MustBeFutureDateError";
            public const string DenyDeleteInUseItem = "DenyDeleteInUseItem";
            public const string DenyDeleteInUseItems = "DenyDeleteInUseItems";
            public const string AnyAutoShipOption = "AnyAutoShipOption";
            public const string CantAddProductToCart = "CantAddProductToCart";
            public const string CartContainsAutoShip = "CartContainsAutoShip";
		    public const string AutoShipNotAvailable = "AutoShipNotAvailable";
		    public const string AccessDenied = "AccessDenied";
		    public const string AutoShipAlreadyStarted = "AutoShipAlreadyStarted";
		    public const string AutoShipAlreadyPaused = "AutoShipAlreadyPaused";
		    public const string AutoShipOrderShouldContainAutoShip = "AutoShipOrderShouldContainAutoShip";
		}

	    public static Dictionary<string, string> Data => new Dictionary<string, string>()
	    {
		    {Keys.UpdateUserGeneral, "Error occurred while updating user"},
		    {Keys.CantFindUserByActivationToken, "Your forgot password link has either already been used or is no longer active. Please use the forgot password system to request a new password recovery link."},
		    {Keys.UserAlreadyConfirmed, "User already activated or deleted"},
		    {Keys.ActivationTokenExpired, "Activation token has expired"},
		    {Keys.UserLockedOut, "User is locked out"},
		    {Keys.IncorrectUserPassword, "Incorrect email or password. Trouble logging in? Please use the password reminder tool to retrieve the password associated with this email account."},
		    {Keys.CantFindUser, "User not found"},
            {Keys.CantFindUserResetPassword, "The email you supplied does not match the profile email that requested this forgot password reset."},
            {Keys.CantFindLogin, "Login information not found"},
		    {Keys.CantSignIn, "User cannot be signed in"},
		    {Keys.UserIsDisabled, "There seems to be an issue accessing your account. Please contact customer care at 800-608-4825 for more information. Thank you."},
            {Keys.EmailIsTakenAlready, "Email '{0}' is already in use"},
            {Keys.AgentIdIsTakenAlready, "Agent ID is already in use"},
			{Keys.CurrentUserRemoval, "Currently logged in user can't be deleted"},
			{Keys.CurrentUserStatusUpdate, "Status of currently logged in user can't be updated"},
			{Keys.GenerateSecurityStampError, "Cannot generate security token"},
			{Keys.UserIsNotConfirmed, "There seems to be an issue accessing your account. Please contact customer care at 800-608-4825 for more information. Thank you."},
			{Keys.CantFindRecord, "Record not found"},
			{Keys.TitleTakenAlready, "Title is already in use"},
			{Keys.LabelTakenAlready, "Label is already in use"},
			{Keys.HasAssignments, "Record cannot be removed because there are references to it"},
			{Keys.AtLeastOneDefaultShipping, "One shipping address should be selected as default"},
			{Keys.AtLeastOneDefaultCreditCard, "One credit card should be selected as default"},
			{Keys.AttemptToUpdateUsingWrongService, "Attempt to update user using forbidden operation"},
			{Keys.AttemptToAssignWrongRole, "Attempt to assign forbidden role"},
			{Keys.IncorrectCustomerRole, "Incorrect customer type"},
			{Keys.EmailIsTakenPleaseLogon, "Email '{0}' is already registered"},
			{Keys.SuspendedCustomer, "Operation can not be completed since associated customer is suspended"},
			{Keys.PasswordRequiresSpecialCharacter, "Passwords must contain at least 1 special character."},
			{Keys.InvalidToken, "Please ensure the email address is identical to the email address where you received the password recovery link. Please use the forgot password system to request a new password recovery link."},
            {Keys.InvalidIdAffiliate, "Affiliate with the given Id cannot be found"},
            {Keys.ObjectNotFound, "Object cannot be found"},
            {Keys.AffiliateMinPayCommisionsAmountNotMatch, "Affiliate balance is less than ${0}"},
            {Keys.WrongCaptcha, "The reCAPTCHA wasn't entered correctly"},
            {Keys.CustomerWasModified, "The customer has been activated by a store front user. Please refresh this page before making changes"},
            {Keys.SkuNotFound, "Product SKU has not been found in the database"},
            {Keys.OrderImportRowError, "Row number {0} error: {1}"},
            {Keys.ParseDateError, "{0} can't be parsed(format 'MM/dd/yyyy')"},
            {Keys.ParseIntError, "{0} can't be parsed"},
            {Keys.ZeroSkusForOrderInImport, "Order should have at least one sku with specified qty"},
            {Keys.InvalidFieldValue, "{0} have invalid value"},
            {Keys.FieldIsRequired, "{0} is required"},
            {Keys.FieldIsInvalidEmail, "{0} is not a valid e-mail address"},
            {Keys.FieldMaxLength, "{0} exceeds length of {1} literals"},
            {Keys.SkuNotFoundOrderImport, "Field \"{0}\": the given product SKU has not been found in the database({1})"},
            {Keys.EmptyCart, "Your cart is empty"},
            {Keys.MustBeFutureDateError,  "{0} should be future date. Please review."},
            {Keys.DenyDeleteInUseItem,  "{0} can't be deleted, because it is in use."},
            {Keys.DenyDeleteInUseItems,  "Some {0} can't be deleted, because there are in use."},
            {Keys.AnyAutoShipOption,  "One of available auto-ship options has to be selected"},
            {Keys.CantAddProductToCart,  "Product can't be added to cart"},
            {Keys.CartContainsAutoShip,  "Your cart contains Auto-Ship product. Please either complete your Auto-Ship order or remove Auto-Ship product from cart before add new product"},
            {Keys.AutoShipNotAvailable,  "Requested Auto-Ship either not available or not found"},
            {Keys.AccessDenied,  "Access denied"},
            {Keys.AutoShipAlreadyStarted,  "Auto-Ship has been started already. Please refresh page to check changes"},
            {Keys.AutoShipAlreadyPaused,  "Auto-Ship has been paused already. Please refresh page to check changes"},
            {Keys.AutoShipOrderShouldContainAutoShip,  "Your Auto-Ship order should contain Auto-Ship product"}
        };
    }
}