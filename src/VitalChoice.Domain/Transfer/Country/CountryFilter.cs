using VitalChoice.Domain.Transfer.Base;

namespace VitalChoice.Domain.Transfer.Country
{
    public class CountryFilter : FilterBase
    {
        public bool ActiveOnly { get; set; }

        public string CountryCode {get;set;}
    }
}