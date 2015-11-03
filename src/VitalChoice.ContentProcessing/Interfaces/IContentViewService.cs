using System.Collections.Generic;
using System.Threading.Tasks;
using VitalChoice.Domain;
using VitalChoice.Domain.Entities.Content;

namespace VitalChoice.ContentProcessing.Interfaces
{
    public interface IContentViewService
    {
        Task<ContentViewModel> GetContentAsync(IDictionary<string, object> queryData);
    }
}