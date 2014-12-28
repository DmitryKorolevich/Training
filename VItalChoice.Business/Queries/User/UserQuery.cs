using VitalChoice.Data.Helpers;

namespace VItalChoice.Business.Queries.User
{
	public class UserQuery: QueryObject<VitalChoice.Entities.Domain.User>
	{
		public UserQuery UserNameStartsWith(string userName)
		{
			Add(x=>x.UserName.StartsWith(userName));
			return this;
		}
	}
}
