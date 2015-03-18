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
	public interface IEcommerceRepositoryAsync<TEntity> : IRepositoryAsync<TEntity> where TEntity : Entity
	{
	}
}
