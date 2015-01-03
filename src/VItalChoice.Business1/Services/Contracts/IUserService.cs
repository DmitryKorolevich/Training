using System.Collections.Generic;
using VitalChoice.Data.Services;
using VitalChoice.Entities.Domain;

namespace VItalChoice.Business.Services.Contracts
{
	public interface IUserService :IGenericService<User>
	{
		IEnumerable<User> QueryByName(string name);
	}
}
