using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Microsoft.Data.Entity.Query;
using VitalChoice.Domain;

namespace VitalChoice.Data.Helpers
{
    public interface IIncludableQueryFluent<TEntity, TPreviousProperty>: IQueryFluent<TEntity> where TEntity : Entity
    {
    }
}