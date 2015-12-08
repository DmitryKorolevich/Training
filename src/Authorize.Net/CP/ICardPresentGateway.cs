using System.Threading.Tasks;
using Authorize.Net.AIM.Requests;
using Authorize.Net.AIM.Responses;

namespace Authorize.Net.CP
{
    public interface ICardPresentGateway
    {
        Task<IGatewayResponse> Send(IGatewayRequest request, string description);
    }
}