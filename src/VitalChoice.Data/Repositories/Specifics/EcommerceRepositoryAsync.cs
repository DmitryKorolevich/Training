using Microsoft.Data.Entity;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using VitalChoice.Data.DataContext;
using VitalChoice.Domain;
using VitalChoice.Domain.Infrastructure;

namespace VitalChoice.Data.Repositories.Specifics
{
	public class EcommerceRepositoryAsync<TEntity> : RepositoryAsync<TEntity> where TEntity : Entity
	{
		public EcommerceRepositoryAsync(IDataContextAsync context) : base(context)
		{
		}
	}
}
