using VitalChoice.Domain;

namespace VitalChoice.Data.Repositories
{
	//added to differentiate uof repository and regular
	public interface IUnitRepositoryAsync<TEntity> : IRepositoryAsync<TEntity> where TEntity : Entity
	{
	}
}
