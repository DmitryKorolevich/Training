using System.Collections.Generic;
using System.Threading.Tasks;

namespace VitalChoice.Data.Services
{
    public interface IObjectLogItemExternalService
    {
        Task LogItems<T>(IEnumerable<T> models, bool logFullObjects = true)
            where T : class;
        Task LogItems<T>(bool logFullObjects, params T[] models)
            where T : class;
        Task LogItem<T>(T model, bool logFullObjects = true)
            where T : class;
    }
}