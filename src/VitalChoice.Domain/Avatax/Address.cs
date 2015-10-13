﻿using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using VitalChoice.Domain.Attributes;

namespace VitalChoice.Domain.Avatax
{
    public enum AddressType
    {
        F, // Firm or company address
        G, // General Delivery address
        H, // High-rise or business complex
        P, // PO box address
        R, // Rural route address
        S // Street or residential address
    }

    public class Address
    {
        // Address can be determined for tax calculation by Line1, City, Region, PostalCode, Country OR Latitude/Longitude OR TaxRegionId
        public string AddressCode { get; set; } // Input for GetTax only, not by address validation

        [Map("Address1")]
        public string Line1 { get; set; }

        [Map("Address2")]
        public string Line2 { get; set; }

        public string Line3 { get; set; }

        [Map]
        public string City { get; set; }

        public string Region { get; set; }

        [Map("Zip")]
        public string PostalCode { get; set; }

        public string Country { get; set; }

        public string County { get; set; } // Output for ValidateAddress only

        public string FipsCode { get; set; } // Output for ValidateAddress only

        public string CarrierRoute { get; set; } // Output for ValidateAddress only

        public string PostNet { get; set; } // Output for ValidateAddress only

        [JsonConverter(typeof(StringEnumConverter))]
        public AddressType? AddressType { get; set; } // Output for ValidateAddress only

        public decimal? Latitude { get; set; } // Input for GetTax only

        public decimal? Longitude { get; set; } // Input for GetTax only

        public string TaxRegionId { get; set; } // Input for GetTax only
    }
}