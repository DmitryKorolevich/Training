using VitalChoice.Domain;

namespace VitalChoice.Data.Repositories.Specifics
{
	public interface ILogsRepositoryAsync<TEntity> : IRepositoryAsync<TEntity> where TEntity : Entity
	{
	}
}
