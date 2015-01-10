using VitalChoice.Data.Helpers;

namespace VItalChoice.Business.Queries.Comment
{
	public class CommentQuery: QueryObject<VitalChoice.Domain.Entities.Comment>
	{
		public CommentQuery CommentStartsWith(string userName)
		{
			Add(x=>x.Text.StartsWith(userName));
			return this;
		}
	}
}
