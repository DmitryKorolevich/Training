using System;
using System.Linq;
using System.Collections.Generic;
using VitalChoice.Data.Repositories;
using VitalChoice.Data.Services;
using VitalChoice.Domain.Entities;
using VitalChoice.Business.Queries.Comment;
using VitalChoice.Business.Queries.User;
using VitalChoice.Business.Services.Contracts;
using VitalChoice.Data.DataContext;
using VitalChoice.Data.UnitOfWork;
using VitalChoice.Domain.Entities.Localization;
using VitalChoice.Infrastructure.UnitOfWork;

namespace VitalChoice.Business.Services.Impl
{
	public class LocalizationService : ILocalizationService
    {
        protected IRepositoryAsync<LocalizationItemData> Repository { get; private set; }

        public LocalizationService(IRepositoryAsync<LocalizationItemData> repository)
        {
            Repository = repository;
        }

	    public string GetString<TEnum>(TEnum enumValue, string cultureId, params object[] args) where TEnum : struct, IComparable, IFormattable
	    {
            var enumType = enumValue.GetType();
            if (!enumType.IsEnum)
            {
                throw new ArgumentException(string.Format("Not enum type {0}.", enumType.FullName));
            }
            var localizationGroupAttribute = enumType.GetCustomAttributes(typeof(LocalizationGroupAttribute), true).SingleOrDefault() as
                LocalizationGroupAttribute;
            if(localizationGroupAttribute==null)
            {
                throw new ArgumentException(string.Format("LocalizationGroupAttribute isn't set on the given enum property {0}.", enumType.FullName));
            }

            //TO DO
            throw new NotImplementedException();
	    }
    }
}
