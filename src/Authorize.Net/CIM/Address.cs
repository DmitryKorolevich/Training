using Authorize.Net.Utility;

namespace Authorize.Net.CIM
{
    /// <summary>
    ///     This is an Address abstraction used for Billing and Shipping
    /// </summary>
    public class Address
    {
        public Address()
        {
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="Address" /> class, resolving the given API Type
        /// </summary>
        /// <param name="fromType">From type.</param>
        public Address(nameAndAddressType fromType)
        {
            City = fromType.city;
            Company = fromType.company;
            Country = fromType.country;
            Last = fromType.lastName;
            First = fromType.firstName;
            Street = fromType.address;
            Zip = fromType.zip;
            State = fromType.state;
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="Address" /> class, resolving the given API Type
        /// </summary>
        /// <param name="fromType">From type.</param>
        public Address(customerAddressType fromType)
        {
            City = fromType.city;
            Company = fromType.company;
            Country = fromType.country;
            Last = fromType.lastName;
            First = fromType.firstName;
            Street = fromType.address;
            Fax = fromType.faxNumber;
            Phone = fromType.phoneNumber;
            Zip = fromType.zip;
            State = fromType.state;
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="Address" /> class, resolving the given API Type
        /// </summary>
        /// <param name="fromType">From type.</param>
        public Address(customerAddressExType fromType)
        {
            City = fromType.city;
            Company = fromType.company;
            Country = fromType.country;
            ID = fromType.customerAddressId;
            Last = fromType.lastName;
            First = fromType.firstName;
            Street = fromType.address;
            Fax = fromType.faxNumber;
            Phone = fromType.phoneNumber;
            State = fromType.state;
            Zip = fromType.zip;
        }

        public string ID { get; set; }
        public string Street { get; set; }
        public string State { get; set; }
        public string City { get; set; }
        public string Country { get; set; }
        public string Zip { get; set; }
        public string Company { get; set; }
        public string Fax { get; set; }
        public string First { get; set; }
        public string Last { get; set; }
        public string Phone { get; set; }

        /// <summary>
        ///     Creates an API type for use with outbound requests to the Gateway. Mostly for internal use.
        /// </summary>
        /// <returns></returns>
        public customerAddressType ToAPIType()
        {
            var result = new customerAddressType
            {
                address = Street,
                city = City,
                company = Company,
                country = Country,
                faxNumber = Fax,
                firstName = First,
                lastName = Last,
                phoneNumber = Phone,
                state = State,
                zip = Zip
            };
            return result;
        }

        /// <summary>
        ///     Creates an API type for use with outbound requests to the Gateway. Mostly for internal use.
        /// </summary>
        /// <returns></returns>
        public customerAddressExType ToAPIExType()
        {
            var result = new customerAddressExType
            {
                address = Street,
                city = City,
                company = Company,
                country = Country,
                faxNumber = Fax,
                firstName = First,
                lastName = Last,
                phoneNumber = Phone,
                state = State,
                zip = Zip,
                customerAddressId = ID
            };
            return result;
        }

        /// <summary>
        ///     Creates an API type for use with outbound requests to the Gateway. Mostly for internal use.
        /// </summary>
        /// <returns></returns>
        public nameAndAddressType ToAPINameAddressType()
        {
            var result = new nameAndAddressType
            {
                address = Street,
                city = City,
                company = Company,
                country = Country,
                firstName = First,
                lastName = Last,
                state = State,
                zip = Zip
            };
            return result;
        }
    }
}