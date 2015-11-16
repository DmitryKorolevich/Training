using System.Collections.Generic;
using System.Threading.Tasks;

namespace VitalChoice.Data.Services
{
    public interface IObjectLogItemExternalService
    {
        Task LogItems(ICollection<object> models, bool logFullObjects);
    }
}