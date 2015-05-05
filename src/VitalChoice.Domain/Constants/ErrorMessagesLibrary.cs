using System;
using System.Collections.Generic;

namespace VitalChoice.Domain.Constants
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
		    public const string CantSignIn = "CanSignIn";
		    public const string UserIsDisabled = "UserIsDisabled";
            public const string EmailIsTakenAlready = "EmailTakenAlready";
            public const string AgentIdIsTakenAlready = "AgentIdIsTakenAlready";
			public const string CurrentUserRemoval = "CurrentUserRemoval";
			public const string CurrentUserStatusUpdate = "CurrentUserStatusUpdate";
		}

	    public static Dictionary<string, string> Data => new Dictionary<string, string>()
	    {
		    {Keys.UpdateUserGeneral, "Error occurred while updating user"},
		    {Keys.CantFindUserByActivationToken, "Unable to find user by activation token"},
		    {Keys.UserAlreadyConfirmed, "User already activated"},
		    {Keys.ActivationTokenExpired, "Activation token has expired"},
		    {Keys.UserLockedOut, "User is locked out"},
		    {Keys.IncorrectUserPassword, "Incorrect email or password"},
		    {Keys.CantFindUser, "User not found"},
		    {Keys.CantSignIn, "User cannot be signed in"},
		    {Keys.UserIsDisabled, "User is disabled by admin"},
            {Keys.EmailIsTakenAlready, "Email '{0}' is already in use."},
            {Keys.AgentIdIsTakenAlready, "Agent ID is already in use."},
			{Keys.CurrentUserRemoval, "Currently logged in user can't be deleted."},
			{Keys.CurrentUserStatusUpdate, "Status of currently logged in user can't be updated."}
		};
    }
}