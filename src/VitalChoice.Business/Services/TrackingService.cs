using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Linq;
using VitalChoice.Data.Repositories.Specifics;
using VitalChoice.Ecommerce.Domain.Entities.Addresses;
using VitalChoice.Infrastructure.Domain.Options;
using VitalChoice.Interfaces.Services;

namespace VitalChoice.Business.Services
{
    public class TrackingService : ITrackingService
    {
        private readonly ILogger _logger;

        public TrackingService(
            ILoggerFactory loggerProvider)
        {
            _logger = loggerProvider.CreateLogger<TrackingService>();
        }

        public string GetServiceUrl(string carrier, string trackingNumber)
        {
            switch (carrier.ToLower())
            {
                case "fedex":
                    return "http://www.fedex.com/Tracking?language=english&tracknumbers=" + trackingNumber;
                case "ups":
                    return
                        "http://wwwapps.ups.com/WebTracking/processInputRequest?TypeOfInquiryNumber=T&InquiryNumber1=" +
                        trackingNumber;
                case "usps":
                    return
                        "http://trkcnfrm1.smi.usps.com/PTSInternetWeb/InterLabelInquiry.do?CAMEFROM=OK&strOrigTrackNum=" +
                        trackingNumber;
                case "ontrac":
                    return "http://www.ontrac.com/tracking.asp?trackingres=submit&tracking_number=" + trackingNumber;
                default:
                    return string.Empty;
            }
        }
    }
}
