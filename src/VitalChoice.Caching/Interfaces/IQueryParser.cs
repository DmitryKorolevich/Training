﻿using System.Linq.Expressions;
using VitalChoice.Caching.Services.Cache.Base;

namespace VitalChoice.Caching.Interfaces
{
    public interface IQueryParser<T>
    {
        QueryData<T> ParseQuery(Expression query);
        IInternalEntityCache<T> InternalEntityCache { get; }
    }
}