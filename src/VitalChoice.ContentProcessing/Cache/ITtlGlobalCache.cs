using System;
using System.Threading.Tasks;
using Templates;

namespace VitalChoice.ContentProcessing.Cache
{
    public interface ITtlGlobalCache : IDisposable

    {
        Task RemoveFromCache(int idMaster, int idContent);

        ITtlTemplate GetOrCreateTemplate(string masterTemplate, string template, DateTime dateUpdated,
            DateTime masterDateUpdated, int idMaster, int idContent);
    }
}