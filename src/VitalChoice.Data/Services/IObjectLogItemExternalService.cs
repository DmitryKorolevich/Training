using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using VitalChoice.Data.Helpers;
using VitalChoice.Domain;
using VitalChoice.Domain.Entities.eCommerce.History;

namespace VitalChoice.Data.Services
{
    public interface IObjectLogItemExternalService
    {
        Task LogItems(ICollection<object> models, bool logFullObjects);
    }
}