using VitalChoice.Ecommerce.Domain;

namespace VitalChoice.Data.Repositories.Specifics
{
	public interface ILogsRepositoryAsync<TEntity> : IRepositoryAsync<TEntity> where TEntity : Entity
	{
	}
}
