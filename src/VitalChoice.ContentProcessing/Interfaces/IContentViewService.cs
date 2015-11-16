using System.Collections.Generic;
using System.Threading.Tasks;
using VitalChoice.Infrastructure.Domain.Content.Base;

namespace VitalChoice.ContentProcessing.Interfaces
{
    public interface IContentViewService
    {
        Task<ContentViewModel> GetContentAsync(IDictionary<string, object> queryData);
    }
}