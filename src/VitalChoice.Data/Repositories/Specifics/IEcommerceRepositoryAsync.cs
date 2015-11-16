using VitalChoice.Ecommerce.Domain;

namespace VitalChoice.Data.Repositories.Specifics
{
	public interface IEcommerceRepositoryAsync<TEntity> : IRepositoryAsync<TEntity> where TEntity : Entity
	{
	}
}
