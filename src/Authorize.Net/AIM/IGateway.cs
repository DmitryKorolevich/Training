using System.Threading.Tasks;
using Authorize.Net.AIM.Requests;
using Authorize.Net.AIM.Responses;

namespace Authorize.Net.AIM
{
    public interface IGateway
    {
        string ApiLogin { get; set; }
        string TransactionKey { get; set; }
        Task<IGatewayResponse> Send(IGatewayRequest request);
        Task<IGatewayResponse> Send(IGatewayRequest request, string description);
    }
}