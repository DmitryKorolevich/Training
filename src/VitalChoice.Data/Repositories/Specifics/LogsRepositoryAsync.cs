using VitalChoice.Data.DataContext;
using VitalChoice.Domain;

namespace VitalChoice.Data.Repositories.Specifics
{
	public class LogsRepositoryAsync<TEntity> : RepositoryAsync<TEntity>, ILogsRepositoryAsync<TEntity> where TEntity : Entity
	{
		public LogsRepositoryAsync(IDataContextAsync context) : base(context)
		{
		}
	}
}
