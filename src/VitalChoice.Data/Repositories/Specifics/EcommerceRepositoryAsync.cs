using VitalChoice.Data.Context;
using VitalChoice.Ecommerce.Domain;

namespace VitalChoice.Data.Repositories.Specifics
{
	public class EcommerceRepositoryAsync<TEntity> : RepositoryAsync<TEntity>, IEcommerceRepositoryAsync<TEntity> where TEntity : Entity
	{
		public EcommerceRepositoryAsync(IDataContextAsync context) : base(context)
		{
		}
	}
}
