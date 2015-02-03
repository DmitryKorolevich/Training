using System;
using System.Collections.Generic;
using VitalChoice.Data.Services;
using VitalChoice.Domain.Entities;
using VitalChoice.Domain.Entities.Localization;

namespace VitalChoice.Business.Services.Contracts
{
	public interface ILocalizationService
	{
	    string GetString<TEnum>(TEnum enumValue) where TEnum : struct, IComparable, IFormattable;

	    string GetString<TEnum>(TEnum enumValue, params object[] args) where TEnum : struct, IComparable, IFormattable;

        string GetDirectString<TEnum>(TEnum enumValue, string cultureId, params object[] args)
	        where TEnum : struct, IComparable, IFormattable;
	}
}
