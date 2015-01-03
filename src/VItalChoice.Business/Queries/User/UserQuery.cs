using VitalChoice.Data.Helpers;

namespace VItalChoice.Business.Queries.User
{
	public class UserQuery: QueryObject<VitalChoice.Domain.Entities.User>
	{
		public UserQuery UserNameStartsWith(string userName)
		{
			Add(x=>x.UserName.StartsWith(userName));
			return this;
		}
	}
}
