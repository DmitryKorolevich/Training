namespace VitalChoice.Infrastructure.Domain.Transfer.Country
{
    public class CountryFilter : FilterBase
    {
        public bool ActiveOnly { get; set; }

        public string CountryCode {get;set;}
    }
}