using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography.Xml;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using CsvHelper;
using CsvHelper.Configuration;
using FluentValidation.Validators;
using Microsoft.Extensions.Logging;
using VitalChoice.Business.CsvImportMaps;
using VitalChoice.Business.Helpers;
using VitalChoice.DynamicData.Interfaces;
using VitalChoice.Ecommerce.Domain.Entities.Addresses;
using VitalChoice.Ecommerce.Domain.Exceptions;
using VitalChoice.Ecommerce.Domain.Helpers;
using VitalChoice.Infrastructure.Domain.Constants;
using VitalChoice.Infrastructure.Domain.Dynamic;
using VitalChoice.Infrastructure.Domain.Entities.Orders;
using VitalChoice.Infrastructure.Domain.Transfer.Country;
using VitalChoice.Interfaces.Services;
using VitalChoice.Interfaces.Services.Settings;
using VitalChoice.Ecommerce.Domain.Entities.Orders;
using VitalChoice.Infrastructure.Domain.Entities;
using VitalChoice.Infrastructure.Domain.Transfer;

namespace VitalChoice.Business.Services.Orders
{
    public abstract class BaseOrderImportProcessor
    {
        protected readonly ICountryNameCodeResolver CountryService;
        protected readonly IDynamicMapper<OrderDynamic, Order> OrderMapper;
        protected readonly IDynamicMapper<AddressDynamic, OrderAddress> AddressMapper;
        protected readonly ReferenceData ReferenceData;
        private readonly ILogger _logger;

        protected abstract Type RecordType { get; }
        protected abstract Type RecordMapType { get; }

        public ICollection<MessageInfo> Messages { get; private set; }

        protected BaseOrderImportProcessor(
            ICountryNameCodeResolver countryService,
            IDynamicMapper<OrderDynamic, Order> orderMapper,
            IDynamicMapper<AddressDynamic, OrderAddress> addressMapper,
            ReferenceData referenceData, ILogger logger)
        {
            CountryService = countryService;
            OrderMapper = orderMapper;
            AddressMapper = addressMapper;
            ReferenceData = referenceData;
            _logger = logger;
            Messages = new List<MessageInfo>();
        }

        protected virtual void ParseAdditionalInfo(OrderBaseImportItem item, CsvReader csv, PropertyInfo[] modelProperties, ref List<MessageInfo> messages)
        {
            
        }

        protected abstract Task<List<OrderImportItemOrderDynamic>> OrdersForImportBaseConvert(List<OrderBaseImportItem> records, OrderImportType orderType,
            CustomerDynamic customer, CustomerPaymentMethodDynamic paymentMethod, int idAddedBy);

        public async Task<IList<OrderImportItemOrderDynamic>> ParseAndValidateAsync(byte[] file, OrderImportType orderType,
            CustomerDynamic customer, CustomerPaymentMethodDynamic paymentMethod, int idAddedBy)
        {
            List<OrderImportItemOrderDynamic> toReturn = null;

            List<OrderBaseImportItem> records = new List<OrderBaseImportItem>();
            Dictionary<string, ImportItemValidationGenericProperty> validationSettings = null;
            using (var memoryStream = new MemoryStream(file))
            {
                using (var streamReader = new StreamReader(memoryStream))
                {
                    CsvConfiguration configuration = new CsvConfiguration();
                    configuration.TrimFields = true;
                    configuration.TrimHeaders = true;
                    configuration.WillThrowOnMissingField = false;
                    configuration.RegisterClassMap(RecordMapType);
                    using (var csv = new CsvReader(streamReader, configuration))
                    {
                        PropertyInfo[] modelProperties = RecordType.GetProperties();
                        validationSettings = BusinessHelper.GetAttrBaseImportValidationSettings(modelProperties);

                        int rowNumber = 1;
                        try
                        {
                            while (csv.Read())
                            {
                                if (rowNumber > FileConstants.MAX_IMPORT_ROWS_COUNT)
                                {
                                    throw new AppValidationException($"File for import cannot contain more than { FileConstants.MAX_IMPORT_ROWS_COUNT}");
                                }

                                OrderBaseImportItem item = (OrderBaseImportItem)csv.GetRecord(RecordType);
                                item.RowNumber = rowNumber;
                                var localMessages = new List<MessageInfo>();
                                rowNumber++;

                                ParseAdditionalInfo(item, csv, modelProperties, ref localMessages);

                                item.ErrorMessages = localMessages;
                                records.Add(item);
                            }
                        }
                        catch (Exception e)
                        {
                            _logger.LogError(e.ToString());
                            throw new AppValidationException(e.Message);
                        }
                    }
                }
            }

            if (validationSettings != null)
            {
                BusinessHelper.ValidateAttrBaseImportItems(records, validationSettings);
            }

            //throw parsing and validation errors
            Messages = BusinessHelper.FormatRowsRecordErrorMessages(records);
            if (Messages.Count > 0)
            {
                throw new AppValidationException(Messages);
            }

            toReturn = await OrdersForImportBaseConvert(records, orderType, customer, paymentMethod, idAddedBy);

            return toReturn;
        }

        protected void ParseContryAndStateCodes(CsvReader reader, string stateColumnName, string countryColumnName, ref List<MessageInfo> messages, out int? idState, out int? idCountry)
        {
            idState = null;
            idCountry = null;
            var stateCode = reader.GetField<string>(stateColumnName);
            var countryCode = reader.GetField<string>(countryColumnName);
            if (!String.IsNullOrEmpty(countryCode))
            {
                var country = CountryService.GetCountryByCode(countryCode);
                if (country != null)
                {
                    idCountry = country.Id;
                    if (!String.IsNullOrEmpty(stateCode))
                    {
                        var state = CountryService.GetStateByCode(country.Id, stateCode);
                        if (state == null)
                        {
                            messages.Add(AddErrorMessage(stateColumnName, String.Format(ErrorMessagesLibrary.Data[ErrorMessagesLibrary.Keys.InvalidFieldValue], stateColumnName)));
                        }
                        else
                        {
                            idState = state.Id;
                        }
                    }
                }
                else
                {
                    messages.Add(AddErrorMessage(countryColumnName, String.Format(ErrorMessagesLibrary.Data[ErrorMessagesLibrary.Keys.InvalidFieldValue], countryColumnName)));
                }
            }
        }

        protected DateTime? ParseOrderShipDate(CsvReader reader, string columnName, ref List<MessageInfo> messages)
        {
            DateTime? toReturn = null;
            DateTime shipDate;
            var sShipDate = reader.GetField<string>(columnName);
            if (!String.IsNullOrEmpty(sShipDate))
            {
                if (DateTime.TryParse(sShipDate, CultureInfo.InvariantCulture, DateTimeStyles.None, out shipDate))
                {
                    toReturn = TimeZoneInfo.ConvertTime(shipDate, TimeZoneHelper.PstTimeZoneInfo, TimeZoneInfo.Local);
                    if (toReturn < DateTime.Now)
                    {
                        messages.Add(AddErrorMessage(columnName, String.Format(ErrorMessagesLibrary.Data[ErrorMessagesLibrary.Keys.MustBeFutureDateError], columnName)));
                    }
                }
                else
                {
                    messages.Add(AddErrorMessage(columnName, String.Format(ErrorMessagesLibrary.Data[ErrorMessagesLibrary.Keys.ParseDateError], columnName)));
                }
            }

            return toReturn;
        }

        protected MessageInfo AddErrorMessage(string field, string message)
        {
            return new MessageInfo()
            {
                Field = field ?? "Base",
                Message = message,
            };
        }
    }
}
