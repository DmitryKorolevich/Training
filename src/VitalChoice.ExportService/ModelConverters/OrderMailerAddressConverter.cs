using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Internal;
using VitalChoice.Business.Services.VeraCore;
using VitalChoice.DynamicData.Base;
using VitalChoice.Infrastructure.Domain.Dynamic;
using VitalChoice.Interfaces.Services;

namespace VitalChoice.ExportService.ModelConverters
{
    public class OrderMailerAddressConverter : BaseModelConverter<OrderMailer, AddressDynamic>
    {
        private readonly ICountryNameCodeResolver _countryNameCodeResolver;

        public OrderMailerAddressConverter(ICountryNameCodeResolver countryNameCodeResolver)
        {
            _countryNameCodeResolver = countryNameCodeResolver;
        }

        public override Task DynamicToModelAsync(OrderMailer model, AddressDynamic dynamic)
        {
            model.Country = _countryNameCodeResolver.GetCountryCode(dynamic);
            model.FirstName = string.Empty;
            model.State = _countryNameCodeResolver.GetRegionOrStateCode(dynamic);
            return TaskCache.CompletedTask;
        }

        public override Task ModelToDynamicAsync(OrderMailer model, AddressDynamic dynamic)
        {
            return TaskCache.CompletedTask;
        }
    }
}