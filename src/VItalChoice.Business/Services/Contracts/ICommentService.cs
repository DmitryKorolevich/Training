using System.Collections.Generic;
using VitalChoice.Data.Services;
using VitalChoice.Domain.Entities;

namespace VitalChoice.Business.Services.Contracts
{
	public interface ICommentService : IGenericService<Comment>
	{
		IEnumerable<Comment> QueryByText(string name);
		void InsertWithUser(Comment comment);
	}
}
