using System.Collections.Generic;
using VitalChoice.Domain.Transfer.Base;

namespace VitalChoice.Business.Services
{
    public interface ILocalizationService
    {
        IList<LookupItem<string>> GetStrings();
        IList<LookupItem<string>> GetStrings(string cultureId);
        string GetString(object enumValue);
        string GetString(object enumValue, params object[] args);
        string GetDirectString(object enumValue, string cultureId, params object[] args);
    }
}