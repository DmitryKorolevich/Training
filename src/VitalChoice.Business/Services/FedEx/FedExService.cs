using Microsoft.Framework.OptionsModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VitalChoice.Business.FedEx.Ship;
using VitalChoice.Domain.Entities.Options;
using VitalChoice.Domain.Entities.VitalGreen;
using VitalChoice.Domain.Exceptions;
using VitalChoice.Interfaces.Services;

namespace VitalChoice.Business.Services.FedEx
{
    public class FedExService : IFedExService
    {
        private readonly FedExOptions _fedExOptions;

        public FedExService(IOptions<AppOptions> appOptions)
        {
            _fedExOptions = appOptions.Options.FedExOptions;
        }

        public string CreateLabel(VitalGreenRequest request, FedExZone zone)
        {
            string toReturn = null;
            CreatePendingShipmentRequest shipRequest = new CreatePendingShipmentRequest
            {
                ClientDetail = new ClientDetail
                {
                    AccountNumber = _fedExOptions.AccountNumber,
                    MeterNumber = _fedExOptions.MeterNumber,
                    Localization = new Localization
                    {
                        LanguageCode = "EN",
                        LocaleCode = "US"
                    }
                },
                RequestedShipment = new RequestedShipment
                {
                    SpecialServicesRequested = new ShipmentSpecialServicesRequested
                    {
                        SpecialServiceTypes = new[]
                        {
                            ShipmentSpecialServiceType.PENDING_SHIPMENT
                        },
                        ReturnShipmentDetail = new ReturnShipmentDetail
                        {
                            ReturnType = ReturnType.PENDING,
                            ReturnEMailDetail = new ReturnEMailDetail
                            {
                                MerchantPhoneNumber = _fedExOptions.MerchantPhoneNumber
                            }
                        },
                        PendingShipmentDetail = new PendingShipmentDetail
                        {
                            Type = PendingShipmentType.EMAIL,
                            ExpirationDate = DateTime.Now.AddDays(2),
                            EmailLabelDetail = new EMailLabelDetail
                            {
                                NotificationEMailAddress = request.Email
                            },
                            ExpirationDateSpecified = true
                        }
                    },
                    DropoffType = DropoffType.REGULAR_PICKUP,
                    PackagingType = PackagingType.YOUR_PACKAGING,
                    ServiceType = zone.State == "HI" ? ServiceType.FEDEX_2_DAY : ServiceType.FEDEX_GROUND,
                    Shipper = new Party
                    {
                        Address = new Address
                        {
                            City = request.City,
                            CountryCode = "US",
                            PostalCode = request.Zip,
                            StateOrProvinceCode = request.State,
                            StreetLines = new[] { request.Address, request.Address2 }
                        },
                        Contact = new Contact
                        {
                            EMailAddress = request.Email,
                            PersonName = request.FirstName + " " + request.LastName,
                            PhoneNumber = request.Phone
                        }
                    },
                    Recipient = new Party
                    {
                        Address = new Address
                        {
                            City = zone.City,
                            CountryCode = "US",
                            PostalCode = zone.Zip,
                            StateOrProvinceCode = zone.State,
                            StreetLines = new[] { zone.Address }
                        },
                        Contact = new Contact
                        {
                            CompanyName = zone.Company,
                            PersonName = zone.Contact,
                            PhoneNumber = zone.Phone
                        }
                    },
                    ShippingChargesPayment = new VitalChoice.Business.FedEx.Ship.Payment
                    {
                        PaymentType = PaymentType.SENDER,
                        Payor = new Payor { AccountNumber = _fedExOptions.PayAccountNumber }
                    },
                    LabelSpecification = new LabelSpecification
                    {
                        LabelFormatType = LabelFormatType.COMMON2D,
                        ImageType = ShippingDocumentImageType.PNG
                    },
                    RateRequestTypes = new[] { RateRequestType.ACCOUNT },
                    PackageCount = "1",
                    RequestedPackageLineItems = new[]
                    {
                        new RequestedPackageLineItem
                        {
                            Weight = new Weight
                            {
                                Units = WeightUnits.LB,
                                Value = 3
                            },
                            PhysicalPackaging = PhysicalPackagingType.BOX,
                            ItemDescription = "Empty Box"
                        }
                    }
                },
                WebAuthenticationDetail = new WebAuthenticationDetail
                {
                    UserCredential = new WebAuthenticationCredential
                    {
                        Key = _fedExOptions.Key,
                        Password = _fedExOptions.Password
                    }
                },
                Version = new VersionId
                {
                    ServiceId = "ship",
                    Major = 10,
                    Intermediate = 0,
                    Minor = 0
                }
            };
            ShipPortTypeClient client = new ShipPortTypeClient();
            var result = client.createPendingShipment(shipRequest);
            if (result.CompletedShipmentDetail != null)
            {
                toReturn = result.CompletedShipmentDetail.AccessDetail.EmailLabelUrl;
            }
            else
            {
                List<MessageInfo> messages = result.Notifications.Select(p => new MessageInfo() { Message = p.Message }).ToList();
                throw new AppValidationException(messages);
            }

            return toReturn;
        }

        public ICollection<VitalGreenDropoffLocation> GetDropoffLocations(VitalGreenRequest request)
        {
            List<VitalGreenDropoffLocation> toReturn = null;
            VitalChoice.Business.FedEx.Locator.FedExLocatorRequest locationsRequest = new VitalChoice.Business.FedEx.Locator.FedExLocatorRequest
            {
                ClientDetail = new VitalChoice.Business.FedEx.Locator.ClientDetail
                {
                    AccountNumber = _fedExOptions.AccountNumber,
                    MeterNumber = _fedExOptions.MeterNumber,
                    Localization = new VitalChoice.Business.FedEx.Locator.Localization
                    {
                        LanguageCode = "EN",
                        LocaleCode = "US"
                    }
                },
                WebAuthenticationDetail = new VitalChoice.Business.FedEx.Locator.WebAuthenticationDetail
                {
                    UserCredential = new VitalChoice.Business.FedEx.Locator.WebAuthenticationCredential
                    {
                        Key = _fedExOptions.Key,
                        Password = _fedExOptions.Password
                    }
                },
                Version = new VitalChoice.Business.FedEx.Locator.VersionId
                {
                    ServiceId = "dloc",
                    Major = 1,
                    Intermediate = 0,
                    Minor = 0
                },
                DistanceUnits = VitalChoice.Business.FedEx.Locator.DistanceUnits.MI,
                NearToAddress = new VitalChoice.Business.FedEx.Locator.Address
                {
                    City = request.City,
                    CountryCode = "US",
                    PostalCode = request.Zip,
                    StateOrProvinceCode = request.State,
                    StreetLines = new[]
                     {
                        request.Address, request.Address2
                    }
                },
                NearToPhoneNumber = request.Phone,
                MaximumMatchCount = "10",
                DropoffServicesDesired = new VitalChoice.Business.FedEx.Locator.DropoffServicesDesired
                {
                    FedExStaffed = true,
                    FedExAuthorizedShippingCenter = true
                }
            };
            VitalChoice.Business.FedEx.Locator.LocatorPortTypeClient client = new VitalChoice.Business.FedEx.Locator.LocatorPortTypeClient();
            var result =client.fedExLocator(locationsRequest);
            if (result.DropoffLocations != null)
            {
                toReturn = result.DropoffLocations.Select
                                    (l => new VitalGreenDropoffLocation
                                    {
                                        BusinessName = l.BusinessName,
                                        Distance = l.Distance.Value.ToString("F2") + l.Distance.Units,
                                        City = l.BusinessAddress.City,
                                        FedExAddress = l.BusinessAddress.StreetLines.Aggregate((current, next) => current + "<br />" + next),
                                        FedExState = l.BusinessAddress.StateOrProvinceCode,
                                        FedExZip = l.BusinessAddress.PostalCode,
                                        Saturdays = l.ServiceProfile.HoursSaturdays,
                                        Weekdays = l.ServiceProfile.HoursWeekdays
                                    }).ToList();
            }
            else
            {
                List<MessageInfo> messages = result.Notifications.Select(p => new MessageInfo() { Message = p.Message }).ToList();
                throw new AppValidationException(messages);
            }

            return toReturn;
        }
    }
}
