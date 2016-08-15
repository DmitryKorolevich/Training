using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using CsvHelper;
using CsvHelper.Configuration;
using FluentValidation.Validators;
using VitalChoice.Business.CsvImportMaps;
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

namespace VitalChoice.Business.Services.Orders
{
    public abstract class BaseOrderImportProcessor
    {
        protected readonly ICountryService CountryService;
        protected readonly IDynamicMapper<OrderDynamic, Order> OrderMapper;
        protected readonly IDynamicMapper<AddressDynamic, OrderAddress> AddressMapper;

        protected abstract Type RecordType { get; }
        protected abstract Type RecordMapType { get; }
        protected ICollection<Country> Countries { get; set; }

        public ICollection<MessageInfo> Messages { get; private set; }

        protected BaseOrderImportProcessor(
            ICountryService countryService,
            IDynamicMapper<OrderDynamic, Order> orderMapper,
            IDynamicMapper<AddressDynamic, OrderAddress> addressMapper)
        {
            CountryService = countryService;
            OrderMapper = orderMapper;
            AddressMapper = addressMapper;
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
            Dictionary<string, OrderValidationGenericProperty> validationSettings = null;
            Countries = await CountryService.GetCountriesAsync(new CountryFilter());
            using (var memoryStream = new MemoryStream(file))
            {
                using (var streamReader = new StreamReader(memoryStream))
                {
                    CsvConfiguration configuration = new CsvConfiguration();
                    configuration.TrimFields = true;
                    configuration.TrimHeaders = true;
                    configuration.RegisterClassMap(RecordMapType);
                    using (var csv = new CsvReader(streamReader, configuration))
                    {
                        PropertyInfo[] modelProperties = RecordType.GetProperties();
                        validationSettings = GetOrderImportValidationSettings(modelProperties);

                        int rowNumber = 1;
                        try
                        {
                            while (csv.Read())
                            {
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
                            throw new AppValidationException("Invalid file format");
                        }
                    }
                }
            }

            if (validationSettings != null)
            {
                ValidateOrderImportItems(records, validationSettings);
            }

            //throw parsing and validation errors
            Messages = FormatRowsRecordErrorMessages(records);
            if (Messages.Count > 0)
            {
                throw new AppValidationException(Messages);
            }

            toReturn = await OrdersForImportBaseConvert(records, orderType, customer, paymentMethod, idAddedBy);

            return toReturn;
        }

        public ICollection<MessageInfo> FormatRowsRecordErrorMessages(IEnumerable<OrderBaseImportItem> items)
        {
            List<MessageInfo> toReturn = new List<MessageInfo>();
            foreach (var item in items)
            {
                toReturn.AddRange(item.ErrorMessages.Select(p => new MessageInfo()
                {
                    Field = p.Field,
                    Message = String.Format(ErrorMessagesLibrary.Data[ErrorMessagesLibrary.Keys.OrderImportRowError], item.RowNumber, p.Message),
                }));
            }
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
                var country = Countries.FirstOrDefault(p => p.CountryCode == countryCode);
                if (country != null)
                {
                    idCountry = country.Id;
                    if (!String.IsNullOrEmpty(stateCode))
                    {
                        var state = country.States.FirstOrDefault(p => p.StateCode == stateCode);
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

        private Dictionary<string, OrderValidationGenericProperty> GetOrderImportValidationSettings(ICollection<PropertyInfo> modelProperties)
        {
            Dictionary<string, OrderValidationGenericProperty> toReturn = new Dictionary<string, OrderValidationGenericProperty>();
            foreach (var modelProperty in modelProperties)
            {
                var displayAttribute = modelProperty.GetCustomAttributes<DisplayAttribute>(true).FirstOrDefault();
                if (displayAttribute != null)
                {
                    OrderValidationGenericProperty item = new OrderValidationGenericProperty();
                    item.DisplayName = displayAttribute.Name;
                    item.PropertyInfo = modelProperty;
                    item.PropertyType = modelProperty.PropertyType;
                    item.Get = modelProperty.GetMethod?.CompileAccessor<object, object>();
                    var requiredAttribute = modelProperty.GetCustomAttributes<RequiredAttribute>(true).FirstOrDefault();
                    if (requiredAttribute != null)
                    {
                        item.IsRequired = true;
                    }
                    var emailAddressAttribute = modelProperty.GetCustomAttributes<EmailAddressAttribute>(true).FirstOrDefault();
                    if (emailAddressAttribute != null)
                    {
                        item.IsEmail = true;
                    }
                    var maxLengthAttribute = modelProperty.GetCustomAttributes<MaxLengthAttribute>(true).FirstOrDefault();
                    if (maxLengthAttribute != null)
                    {
                        item.MaxLength = maxLengthAttribute.Length;
                    }
                    toReturn.Add(modelProperty.Name, item);
                }
            }

            return toReturn;
        }

        private void ValidateOrderImportItems(ICollection<OrderBaseImportItem> models, Dictionary<string, OrderValidationGenericProperty> settings)
        {
            EmailValidator emailValidator = new EmailValidator();
            var emailRegex = new Regex(emailValidator.Expression, RegexOptions.IgnoreCase);
            foreach (var model in models)
            {
                foreach (var pair in settings)
                {
                    var setting = pair.Value;

                    bool valid = true;
                    if (typeof(string) == setting.PropertyType)
                    {
                        string value = (string)setting.Get(model);
                        if (setting.IsRequired && String.IsNullOrEmpty(value))
                        {
                            model.ErrorMessages.Add(AddErrorMessage(setting.DisplayName, String.Format(ErrorMessagesLibrary.Data[ErrorMessagesLibrary.Keys.FieldIsRequired], setting.DisplayName)));
                            valid = false;
                        }

                        if (valid && setting.MaxLength.HasValue && !String.IsNullOrEmpty(value) && value.Length > setting.MaxLength.Value)
                        {
                            model.ErrorMessages.Add(AddErrorMessage(setting.DisplayName, String.Format(ErrorMessagesLibrary.Data[ErrorMessagesLibrary.Keys.FieldMaxLength], setting.DisplayName, setting.MaxLength.Value)));
                            valid = false;
                        }

                        if (valid && setting.IsEmail && !String.IsNullOrEmpty(value))
                        {
                            if (!emailRegex.IsMatch(value))
                            {
                                model.ErrorMessages.Add(AddErrorMessage(setting.DisplayName, String.Format(ErrorMessagesLibrary.Data[ErrorMessagesLibrary.Keys.FieldIsInvalidEmail], setting.DisplayName)));
                            }
                        }
                    }
                }
            }
        }
    }
}
