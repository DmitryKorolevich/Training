using VitalChoice.Data.Helpers;

namespace VItalChoice.Business.Queries.User
{
	public class UserQuery: QueryObject<User>
	{
		public UserQuery UserNameStartsWith(string userName)
		{
			Add(x=>x.UserName.StartsWith(userName));
			return this;
		}
	}
}
