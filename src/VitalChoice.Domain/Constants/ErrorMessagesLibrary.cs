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
			public const string GenerateSecurityStampError = "GenerateSecurityStampError";
			public const string UserIsNotConfirmed = "UserIsNotConfirmed";
		    public const string CantFindRecord = "CantFindRecord";
		    public const string TitleTakenAlready = "TitleTakenAlready";
		    public const string LabelTakenAlready = "LabelTakenAlready";
		    public const string HasAssignments = "HasAssignments";
		    public const string AtLeastOneDefaultShipping = "AtLeastOneDefaultShipping";
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
			{Keys.AtLeastOneDefaultShipping, "At least one shipping address should be default"}
		};
    }
}