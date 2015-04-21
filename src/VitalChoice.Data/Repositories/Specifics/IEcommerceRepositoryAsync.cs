using VitalChoice.Domain;

namespace VitalChoice.Data.Repositories.Specifics
{
	public interface IEcommerceRepositoryAsync<TEntity> : IRepositoryAsync<TEntity> where TEntity : Entity
	{
	}
}
