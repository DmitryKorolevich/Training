using VitalChoice.Domain.Transfer;
using VitalChoice.Domain.Transfer.Base;

namespace VitalChoice.Business.Services.Contracts
{
	public interface IAppInfrastructureService
	{
		ReferenceData Get();
	}
}
