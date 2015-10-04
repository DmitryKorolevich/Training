using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using VitalChoice.Domain.Constants;

namespace VitalChoice.Infrastructure.Identity
{
    public class ExtendedIdentityErrorDescriber: IdentityErrorDescriber
    {
	    public override IdentityError DuplicateEmail(string email)
	    {
		    var result = base.DuplicateEmail(email);
			result.Description = string.Format(ErrorMessagesLibrary.Data[ErrorMessagesLibrary.Keys.EmailIsTakenAlready], email);

		    return result;
	    }

		public override IdentityError DuplicateUserName(string email)
		{
			var result = base.DuplicateUserName(email);
			result.Description = string.Format(ErrorMessagesLibrary.Data[ErrorMessagesLibrary.Keys.EmailIsTakenAlready], email);

			return result;
		}
	}
}
