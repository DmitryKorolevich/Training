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
		    {Keys.CantSignIn, "User cannot be signed in"}
	    };
    }
}