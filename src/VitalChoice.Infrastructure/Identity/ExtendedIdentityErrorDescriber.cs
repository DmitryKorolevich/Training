using Microsoft.AspNetCore.Identity;
using VitalChoice.Infrastructure.Domain.Constants;

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

        public override IdentityError PasswordRequiresNonAlphanumeric()
        {
            var result = base.PasswordRequiresNonAlphanumeric();
            result.Description = ErrorMessagesLibrary.Data[ErrorMessagesLibrary.Keys.PasswordRequiresSpecialCharacter];
            return result;
        }

	    public override IdentityError InvalidToken()
	    {
			var result = base.InvalidToken();
			result.Description = ErrorMessagesLibrary.Data[ErrorMessagesLibrary.Keys.InvalidToken];

			return result;
		}
    }
}
