using System.Collections.Generic;
using System.Threading.Tasks;
using Authorize.Net.Api.Contracts.V1;

namespace Authorize.Net.Api.Controllers.Bases
{
    /**
     * @author ramittal
     *
     */
#pragma warning disable 1591
    public interface IApiOperation<TQ, TS>
        where TQ : ANetApiRequest
        where TS : ANetApiResponse
    {
        TS GetApiResponse();
        ANetApiResponse GetErrorResponse();
        Task<TS> ExecuteWithApiResponse(Environment environment = null);
        Task Execute(Environment environment = null);
        messageTypeEnum GetResultCode();
        List<string> GetResults();
    }
#pragma warning restore 1591
}