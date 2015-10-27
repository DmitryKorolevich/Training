using System;
using System.Collections.Generic;

namespace VitalChoice.Domain.Constants
{
    public static class InfoMessagesLibrary
    {
	    public static class Keys
	    {
		    public const string EntitySuccessfullyAdded = "EntitySuccessfullyAdded";
		    public const string EntitySuccessfullyUpdated = "EntitySuccessfullyUpdated";
		}

	    public static Dictionary<string, string> Data => new Dictionary<string, string>()
	    {
		    {Keys.EntitySuccessfullyAdded, "Successfully added"},
		    {Keys.EntitySuccessfullyUpdated, "Successfully updated"},
		};
    }
}